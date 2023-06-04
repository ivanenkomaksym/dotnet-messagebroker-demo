using Common.Events;
using Common.Models.Warehouse;
using MassTransit;
using System.Diagnostics;
using System.Text.Json;
using WarehouseCommon.Repositories;

namespace Warehouse.Consumers
{
    internal class ReserveStockConsumer : IConsumer<ReserveStock>
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<ReserveStockConsumer> _logger;

        public ReserveStockConsumer(IWarehouseRepository warehouseRepository, IPublishEndpoint publishEndpoint, ILogger<ReserveStockConsumer> logger)
        {
            _warehouseRepository = warehouseRepository;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReserveStock> context)
        {
            // In
            var reserveStock = context.Message;
            var message = JsonSerializer.Serialize(reserveStock);
            _logger.LogInformation($"Received `ReserveStock` event with content: {message}");

            // TODO: Implement rollback in case of failure - remove reserve and restore AvailableOnStock
            var orderReserve = await CreateReserveAndUpdateStock(reserveStock);

            // Out
            var stockReservedEvent = new StockReserved
            {
                OrderReserveId = orderReserve.Id,
                OrderId = orderReserve.OrderId,
                ReservedStockItems = orderReserve.ReservedStockItems
            };

            await _publishEndpoint.Publish(stockReservedEvent);

            message = JsonSerializer.Serialize(stockReservedEvent);
            _logger.LogInformation($"Sent `StockReserved` event with content: {message}");
        }

        private async Task<OrderReserve> CreateReserveAndUpdateStock(ReserveStock reserveStock)
        {
            // TODO: To be moved to WarehouseRepository itself and made atomic operation
            var reserveStockItems = new List<ReservedStockItem>();
            foreach (var item in reserveStock.Items)
            {
                var stockItem = await _warehouseRepository.GetStockItemByProductId(item.ProductId);

                reserveStockItems.Add(new ReservedStockItem
                {
                    Id = Guid.NewGuid(),
                    StockItemId = stockItem.Id,
                    Reserved = item.Quantity
                });

                // TODO: Send failure message in case there are less items available on stock than requested to reserve
                Debug.Assert(stockItem.AvailableOnStock > item.Quantity);
                stockItem.AvailableOnStock -= item.Quantity;
                await _warehouseRepository.UpdateStockItem(stockItem);
            }

            return await _warehouseRepository.CreateOrderReserve(new OrderReserve
            {
                Id = Guid.NewGuid(),
                OrderId = reserveStock.OrderId,
                ReservedStockItems = reserveStockItems
            });
        }
    }
}