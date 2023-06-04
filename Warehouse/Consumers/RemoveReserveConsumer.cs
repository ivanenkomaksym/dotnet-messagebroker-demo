using System.Text.Json;
using Common.Events;
using WarehouseCommon.Repositories;
using MassTransit;
using System.Diagnostics;

namespace Warehouse.Consumers
{
    internal class RemoveReserveConsumer : IConsumer<RemoveReserve>
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<RemoveReserveConsumer> _logger;

        public RemoveReserveConsumer(IWarehouseRepository warehouseRepository, IPublishEndpoint publishEndpoint, ILogger<RemoveReserveConsumer> logger)
        {
            _warehouseRepository = warehouseRepository;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RemoveReserve> context)
        {
            // In
            var removeReserve = context.Message;
            var message = JsonSerializer.Serialize(removeReserve);
            _logger.LogInformation($"Received `RemoveReserve` event with content: {message}");

            if (removeReserve.Reason == RemoveReserveReason.TakeFromStockForShipment)
            {
                var result = await _warehouseRepository.DeleteReserve(removeReserve.OrderId);
                Debug.Assert(result);
            }
            else
            {
                // If RemoveReserveReason is different than TakeFromStockForShipment, increase StockItems by number that was originally requested to be reserved
                await RemoveReserveAndUpdateStock(removeReserve.OrderId);
            }

            // Out
            var reserveRemovedEvent = new ReserveRemoved
            {
                OrderId = removeReserve.OrderId,
            };

            await _publishEndpoint.Publish(reserveRemovedEvent);

            message = JsonSerializer.Serialize(reserveRemovedEvent);
            _logger.LogInformation($"Sent `ReserveRemoved` event with content: {message}");
        }

        private async Task RemoveReserveAndUpdateStock(Guid orderId)
        {
            // TODO: To be moved to WarehouseRepository itself and made atomic operation
            var reserve = await _warehouseRepository.GetOrderReserveByOrderId(orderId);
            foreach (var reservedStockItem in reserve.ReservedStockItems)
            {
                var stockItem = await _warehouseRepository.GetStockItemById(reservedStockItem.StockItemId);

                stockItem.AvailableOnStock += reservedStockItem.Reserved;
                await _warehouseRepository.UpdateStockItem(stockItem);
            }

            await _warehouseRepository.DeleteReserve(orderId);
        }
    }
}
