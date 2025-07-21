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

        private readonly object NotImplementedDetailMessage = new
        {
            Message = "Search functionality is not available in this environment. Please use MongoDB Atlas for search capabilities.",
        };

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

        /// <summary>
        /// Authocomplete
        /// </summary>
        /// <param name="query">Query to search</param>
        /// <returns></returns>
        [HttpGet("autocomplete/{query}", Name = "autocomplete")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotImplemented)]
        public async Task<ActionResult<IEnumerable<Product>>> AutocompleteProductsAsync(string query)
        {
            try
            {
                var products = await _catalogApiService.Autocomplete(query);
                ArgumentNullException.ThrowIfNull(products);
                return products.ToList();
            }
            catch (NotImplementedException)
            {
                return StatusCode(501, NotImplementedDetailMessage);
            }
        }

        /// <summary>
        /// Finds products semantically relevant to the input text using vector search.
        /// </summary>
        /// <param name="text">The input text for semantic search.</param>
        /// <returns>A list of semantically relevant products.</returns>
        [HttpGet("findwithsemanticrelevance/{text}", Name = "findwithsemanticrelevance")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)] // Potentially from fallback
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)] // For embedding generation failure
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)] // For unexpected errors
        [ProducesResponseType((int)HttpStatusCode.NotImplemented)]
        public async Task<ActionResult<IEnumerable<Product>>> FindWithSemanticRelevanceAsync(string text)
        {
            try
            {
                var products = await _catalogApiService.FindWithSemanticRelevance(text);
                ArgumentNullException.ThrowIfNull(products);
                return products.ToList();
            }
            catch (NotImplementedException)
            {
                return StatusCode(501, NotImplementedDetailMessage);
            }
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