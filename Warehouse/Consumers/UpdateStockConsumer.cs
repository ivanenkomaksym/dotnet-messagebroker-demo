using Common.Events;
using MassTransit;
using System.Text.Json;
using WarehouseCommon.Repositories;

namespace Warehouse.Consumers
{
    internal class UpdateStockConsumer : IConsumer<UpdateStock>
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<UpdateStockConsumer> _logger;

        public UpdateStockConsumer(IWarehouseRepository warehouseRepository, IPublishEndpoint publishEndpoint, ILogger<UpdateStockConsumer> logger)
        {
            _warehouseRepository = warehouseRepository;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UpdateStock> context)
        {
            // In
            var updateStock = context.Message;
            var message = JsonSerializer.Serialize(updateStock);
            _logger.LogInformation($"Received `UpdateStock` event with content: {message}");

            var updatedStockItemIds = await UpdateStockItems(updateStock);

            // Out
            var stockUpdatedEvent = new StockUpdated
            {
                StockItemIds = updatedStockItemIds
            };

            await _publishEndpoint.Publish(stockUpdatedEvent);

            message = JsonSerializer.Serialize(stockUpdatedEvent);
            _logger.LogInformation($"Sent `StockUpdated` event with content: {message}");
        }

        private async Task<IList<Guid>> UpdateStockItems(UpdateStock updateStock)
        {
            var updatedStockItemIds = new List<Guid>();
            foreach (var item in updateStock.Items)
            {
                var stockItem = await _warehouseRepository.GetStockItemByProductId(item.ProductId);

                stockItem.AvailableOnStock += item.Quantity;
                stockItem.Sold -= item.Quantity;

                await _warehouseRepository.UpdateStockItem(stockItem);
                updatedStockItemIds.Add(stockItem.Id);
            }

            return updatedStockItemIds;
        }
    }
}
