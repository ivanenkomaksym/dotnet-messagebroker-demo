using Common.Extensions;
using Common.Models;
using System.Text;
using System.Text.Json;

namespace WebUI.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _client;

        public ProductService(HttpClient client, ILogger<ProductService> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<ProductWithStock>> GetProducts()
        {
            var response = await _client.GetAsync("/gateway/Products");
            return await response.ReadContentAs<List<ProductWithStock>>();
        }

        public async Task<ProductWithStock> GetProduct(Guid productId)
        {
            var response = await _client.GetAsync($"/gateway/Products/{productId}");
            return await response.ReadContentAs<ProductWithStock>();
        }

        public async Task<IEnumerable<ProductWithStock>> GetProductsByCategory(string category)
        {
            var response = await _client.GetAsync($"/gateway/Products/GetProductByCategory/{category}");
            return await response.ReadContentAs<List<ProductWithStock>>();
        }

        public async Task<ProductWithStock> CreateProductWithStock(ProductWithStock productWithStock)
        {
            var content = new StringContent(JsonSerializer.Serialize(productWithStock), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/gateway/Products", content);
            response.EnsureSuccessStatusCode();

            var createdProduct = await response.Content.ReadFromJsonAsync<ProductWithStock>();
            return createdProduct;
        }

        public async Task<ProductWithStock> UpdateProductWithStock(ProductWithStock productWithStock)
        {
            var content = new StringContent(JsonSerializer.Serialize(productWithStock), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/gateway/Products", content);
            response.EnsureSuccessStatusCode();

            var updatedProduct = await response.Content.ReadFromJsonAsync<ProductWithStock>();
            return updatedProduct;
        }

        public async Task DeleteProduct(Guid productId)
        {
            var response = await _client.DeleteAsync($"/gateway/Products/{productId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
