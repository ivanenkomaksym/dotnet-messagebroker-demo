using System.Net;
using Common.Models;
using Common.Models.Warehouse;
using Microsoft.AspNetCore.Mvc;
using WebUIAggregatorAPI.Services;

namespace WebUIAggregatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ICatalogApiService _catalogApiService;
        private readonly IWarehouseApiService _warehouseApiService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ICatalogApiService catalogApiService, IWarehouseApiService warehouseApiService, ILogger<ProductsController> logger)
        {
            _catalogApiService = catalogApiService;
            _warehouseApiService = warehouseApiService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET /api/products
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductWithStock>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _catalogApiService.GetProducts();
            ArgumentNullException.ThrowIfNull(products);
            var productsWithStock = await AddStockInformation(products);

            return Ok(productsWithStock);
        }

        // GET /api/products/{productId}
        [HttpGet("{productId}")]
        [ProducesResponseType(typeof(ProductWithStock), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProduct(Guid productId)
        {
            var product = await _catalogApiService.GetProduct(productId);
            if (product == null)
            {
                return NotFound();
            }

            var productWithStock = await AddStockInformation(product);

            return Ok(productWithStock);
        }

        // GET /api/products/GetProductByCategory/{category}
        [HttpGet("GetProductByCategory/{category}")]
        [ProducesResponseType(typeof(IEnumerable<ProductWithStock>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProductByCategory(string category)
        {
            var products = await _catalogApiService.GetProductsByCategory(category);
            ArgumentNullException.ThrowIfNull(products);
            var productsWithStock = await AddStockInformation(products);

            return Ok(productsWithStock);
        }

        // POST /api/products
        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateProduct(ProductWithStock productWithStock)
        {
            // Perform necessary validation or additional operations
            var product = new Product
            {
                Id = productWithStock.Id,
                Name = productWithStock.Name,
                Category = productWithStock.Category,
                Summary = productWithStock.Summary,
                ImageFile = productWithStock.ImageFile,
                Price = productWithStock.Price
            };

            var createdProduct = await _catalogApiService.CreateProduct(product);
            ArgumentNullException.ThrowIfNull(createdProduct);

            var stockItem = new StockItem
            {
                Id = productWithStock.StockItemId,
                ProductId = createdProduct.Id,
                ProductName = createdProduct.Name,
                Supplier = "ABC LTD",
                AvailableOnStock = productWithStock.AvailableOnStock,
                Discount = productWithStock.Discount,
                Price = productWithStock.Price,
                Quantity = productWithStock.Quantity,
                Sold = productWithStock.Sold
            };

            var createdStockItem = await _warehouseApiService.CreateStockItem(stockItem);

            return CreatedAtAction(nameof(GetProduct), new { productId = createdProduct.Id }, createdProduct);
        }

        // PUT /api/products
        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProduct(ProductWithStock productWithStock)
        {
            // Perform necessary validation or additional operations
            var product = new Product
            {
                Id = productWithStock.Id,
                Name = productWithStock.Name,
                Category = productWithStock.Category,
                Summary = productWithStock.Summary,
                ImageFile = productWithStock.ImageFile,
                Price = productWithStock.Price
            };
            var updatedProduct = await _catalogApiService.UpdateProduct(product);

            var stockItem = new StockItem
            {
                Id = productWithStock.StockItemId,
                ProductId = product.Id,
                ProductName = product.Name,
                Supplier = "ABC LTD",
                AvailableOnStock = productWithStock.AvailableOnStock,
                Discount = productWithStock.Discount,
                Price = productWithStock.Price,
                Quantity = productWithStock.Quantity,
                Sold = productWithStock.Sold
            };
            var updatedStockItem = await _warehouseApiService.UpdateStockItem(stockItem);

            return Ok(updatedProduct);
        }

        // DELETE /api/products/{productId}
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            await _catalogApiService.DeleteProduct(productId);
            await _warehouseApiService.DeleteStockItemByProductId(productId);

            return NoContent();
        }

        private async Task<IEnumerable<ProductWithStock>> AddStockInformation(IEnumerable<Product> products)
        {
            var productsWithStock = new List<ProductWithStock>();

            foreach (var product in products)
            {
                var productWithStock = await AddStockInformation(product);

                productsWithStock.Add(productWithStock);
            }

            return productsWithStock;
        }

        private async Task<ProductWithStock> AddStockInformation(Product product)
        {
            var stockItem = await _warehouseApiService.GetStockItemByProductId(product.Id);
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

            return productWithStock;
        }
    }
}