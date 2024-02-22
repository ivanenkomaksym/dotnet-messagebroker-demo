using Common.Models;
using Common.Models.Warehouse;

namespace Common.SeedData
{
    public static class WarehouseSeed
    {
        public static IEnumerable<StockItem> GetPreconfiguredStockItems(IEnumerable<Product> products)
        {
            var stockItems = new List<StockItem>();
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

            return stockItems;
        }
    }
}
