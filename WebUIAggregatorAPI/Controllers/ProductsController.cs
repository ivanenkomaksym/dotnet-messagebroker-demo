using Common.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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
            var productsWithStock = await AddStockInformation(products);

            return Ok(productsWithStock);
        }

        // POST /api/products
        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            // Perform necessary validation or additional operations
            var createdProduct = await _catalogApiService.CreateProduct(product);

            return CreatedAtAction(nameof(GetProduct), new { productId = createdProduct.Id }, createdProduct);
        }

        // PUT /api/products
        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            // Perform necessary validation or additional operations
            var updatedProduct = await _catalogApiService.UpdateProduct(product);

            return Ok(updatedProduct);
        }

        // DELETE /api/products/{productId}
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            await _catalogApiService.DeleteProduct(productId);
            // Also delete any related stock information from the WarehouseAPI if necessary

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
                ImageFile = product.ImageFile,
                Price = product.Price,
                Quantity = stockItem?.Quantity ?? 0,
                Discount = stockItem?.Discount ?? 0,
                Sold = stockItem?.Sold ?? 0,
                AvailableOnStock = stockItem?.AvailableOnStock ?? 0
            };

            return productWithStock;
        }
    }
}
