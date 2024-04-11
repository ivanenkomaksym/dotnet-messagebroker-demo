using System.Text.Json;
using Common.Events;
using Common.Models.Warehouse;
using WarehouseCommon.Repositories;
using MassTransit;

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

        public Task Consume(ConsumeContext<ReserveStock> context)
        {
            // In
            var reserveStock = context.Message;
            var message = JsonSerializer.Serialize(reserveStock);
            _logger.LogInformation($"Received `ReserveStock` event with content: {message}");

            _ = ScheduleReserveStock(reserveStock);

            return Task.CompletedTask;
        }

        private Task ScheduleReserveStock(ReserveStock reserveStock)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(10 * 1000); // wait 10s

                // TODO: Implement rollback in case of failure - remove reserve and restore AvailableOnStock
                var reserveStockResult = await CreateReserveAndUpdateStock(reserveStock);

                // Out
                await _publishEndpoint.Publish(reserveStockResult);

                var message = JsonSerializer.Serialize(reserveStockResult);
                _logger.LogInformation($"Sent `ReserveStockResult` event with content: {message}");
            });
        }

        private async Task<ReserveStockResult> CreateReserveAndUpdateStock(ReserveStock reserveStock)
        {
            // First check if stock levels allow reserving the order items
            var failedToReserveProducts = new List<FailedToReserveProduct>();
            foreach (var item in reserveStock.Items)
            {
                var stockItem = await _warehouseRepository.GetStockItemByProductId(item.ProductId);

                // TODO: Send failure message in case there are less items available on stock than requested to reserve
                if (stockItem.AvailableOnStock < item.Quantity)
                {
                    failedToReserveProducts.Add(new FailedToReserveProduct
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        AvailableOnStock = stockItem.AvailableOnStock
                    });
                }
            }

            // If there is at least one item that exceeds stock levels, return failure result
            if (failedToReserveProducts.Count > 0)
            {
                return new ReserveStockResult
                {
                    OrderId = reserveStock.OrderId,
                    CustomerInfo = reserveStock.CustomerInfo,
                    ReserveResult = ReserveResult.Failed,
                    FailedToReserveProducts = failedToReserveProducts
                };
            }

            // Otherwise apply stock reserve
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

                stockItem.AvailableOnStock -= item.Quantity;
                await _warehouseRepository.UpdateStockItem(stockItem);
            }

            var orderReserve = await _warehouseRepository.CreateOrderReserve(new OrderReserve
            {
                Id = Guid.NewGuid(),
                OrderId = reserveStock.OrderId,
                ReservedStockItems = reserveStockItems
            });

            return new ReserveStockResult
            {
                OrderReserveId = orderReserve.Id,
                OrderId = orderReserve.OrderId,
                CustomerInfo = reserveStock.CustomerInfo,
                ReserveResult = ReserveResult.Reserved,
                ReservedStockItems = orderReserve.ReservedStockItems
            };
        }
    }
}