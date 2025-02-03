using Common.Models.Warehouse;
using Common.SeedData;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace WarehouseCommon.Data
{
    /// <summary>
    /// This class seeds warehouse data.
    /// </summary>
    public sealed class WarehouseContextSeed : IWarehouseContextSeed
    {
        private readonly ILogger<WarehouseContextSeed> _logger;

        public WarehouseContextSeed(ILogger<WarehouseContextSeed> logger)
        {
            _logger = logger;
        }

        public async Task SeedData(IMongoCollection<StockItem> stockItemCollection)
        {
            var hasAtLeastOneStockItem = stockItemCollection.Find(p => true).Any();
            if (hasAtLeastOneStockItem)
                return;

            _logger.LogInformation("SeedData started.");
            var stockItems = new List<StockItem>();
            var products = CatalogSeed.GetPreconfiguredProducts();
            _logger.LogInformation($"Received `{products.Count()}` products.");
            var rand = new Random();

            foreach (var product in products)
            {
                var quantity = (ushort)rand.Next(20, 100);
                var sold = (ushort)rand.Next(0, quantity);
                var availableOnStock = (ushort)(quantity - sold);

                stockItems.Add(new StockItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Supplier = "ABC LTD",
                    Quantity = quantity,
                    Price = product.Price,
                    Discount = (decimal)Math.Round(rand.NextDouble() * 0.2, 2),
                    Sold = sold,
                    AvailableOnStock = availableOnStock
                });
            }

            bool existProduct = stockItemCollection.Find(p => true).Any();
            if (!existProduct)
            {
                await stockItemCollection.InsertManyAsync(stockItems);
            }
            _logger.LogInformation("SeedData ended.");
        }
    }
}