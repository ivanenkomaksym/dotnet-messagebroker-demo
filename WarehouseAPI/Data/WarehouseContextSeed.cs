using Common.Extensions;
using Common.Models.Warehouse;
using MongoDB.Driver;

namespace WarehouseAPI.Data
{
    /// <summary>
    /// This class is at the same time HttpClient in order to get available products from CatalogAPI and seed warehouse data.
    /// </summary>
    internal sealed class WarehouseContextSeed : IWarehouseContextSeed
    {
        private readonly HttpClient _client;
        private readonly ILogger<WarehouseContextSeed> _logger;

        public WarehouseContextSeed(HttpClient client, ILogger<WarehouseContextSeed> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task SeedData(IMongoCollection<StockItem> stockItemCollection)
        {
            _logger.LogInformation("SeedData started.");
            var stockItems = new List<StockItem>();
            var products = await GetCatalog();
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
                    Discount = Math.Round(rand.NextDouble() * 0.2, 2),
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

        public async Task<IEnumerable<CatalogModel>> GetCatalog()
        {
            var response = await _client.GetAsync("/gateway/Catalog");
            return await response.ReadContentAs<List<CatalogModel>>();
        }
    }
}
