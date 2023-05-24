using Common.Extensions;
using Common.Models.Warehouse;
using MongoDB.Driver;

namespace Warehouse.Data
{
    /// <summary>
    /// This class is at the same time HttpClient in order to get available products from CatalogAPI and seed warehouse data.
    /// </summary>
    internal sealed class WarehouseContextSeed : IWarehouseContextSeed
    {
        private readonly HttpClient _client;

        public WarehouseContextSeed(HttpClient client)
        {
            _client = client;
        }

        public async Task SeedData(IMongoCollection<StockItem> stockItemCollection)
        {
            var stockItems = new List<StockItem>();
            var products = await GetCatalog();
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
                    Discount = (decimal)(rand.NextDouble() * 0.2),
                    Sold = sold,
                    AvailableOnStock = availableOnStock
                });
            }

            bool existProduct = stockItemCollection.Find(p => true).Any();
            if (!existProduct)
            {
                stockItemCollection.InsertManyAsync(stockItems);
            }
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalog()
        {
            var response = await _client.GetAsync("/gateway/Catalog");
            return await response.ReadContentAs<List<CatalogModel>>();
        }
    }
}
