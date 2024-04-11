using Common.Models;
using Common.Models.Warehouse;
using Common.SeedData;
using Microsoft.CodeAnalysis;

namespace WebUI.Services
{
    public class StubProductService : IProductService
    {
        private static readonly IEnumerable<Product> Products = CatalogSeed.GetPreconfiguredProducts();
        private static readonly IEnumerable<StockItem> StockItems = WarehouseSeed.GetPreconfiguredStockItems(Products);

        private async Task<IEnumerable<ProductWithStock>?> AddStockInformation(IEnumerable<Product> products)
        {
            var productsWithStock = new List<ProductWithStock>();

            foreach (var product in products)
            {
                var productWithStock = await AddStockInformation(product);
                if (productWithStock != null)
                    productsWithStock.Add(productWithStock);
            }

            return productsWithStock;
        }

        private Task<ProductWithStock?> AddStockInformation(Product product)
        {
            var stockItem = StockItems.Where(stockItem => stockItem.ProductId == product.Id).FirstOrDefault();
            var productWithStock = new ProductWithStock
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Summary = product.Summary,
                ImageFile = product.ImageFile,
                Price = product.Price,
                StockItemId = stockItem?.Id ?? Guid.Empty,
                Quantity = stockItem?.Quantity ?? 0,
                Discount = stockItem?.Discount ?? 0,
                Sold = stockItem?.Sold ?? 0,
                AvailableOnStock = stockItem?.AvailableOnStock ?? 0
            };

            return Task.FromResult<ProductWithStock?>(productWithStock);
        }

        public Task<ProductWithStock?> CreateProductWithStock(ProductWithStock productWithStock)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProduct(Guid productId)
        {
            throw new NotImplementedException();
        }

        public Task<ProductWithStock?> GetProduct(Guid productId)
        {
            var product = Products.Where(p => p.Id == productId).FirstOrDefault();
            ArgumentNullException.ThrowIfNull(product);
            var productWithStock = AddStockInformation(product);
            return productWithStock;
        }

        public Task<IEnumerable<ProductWithStock>?> GetProducts()
        {
            var productsWithStock = AddStockInformation(Products);
            return productsWithStock;
        }

        public Task<IEnumerable<ProductWithStock>?> GetProductsByCategory(string category)
        {
            var products = Products.Where(p => p.Category == category);
            var productsWithStock = AddStockInformation(products);
            return productsWithStock;
        }

        public Task<ProductWithStock?> UpdateProductWithStock(ProductWithStock productWithStock)
        {
            throw new NotImplementedException();
        }
    }
}
