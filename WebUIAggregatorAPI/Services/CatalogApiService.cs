using System.Net;
using System.Text.Json;
using System.Text;
using Common.Models;

namespace WebUIAggregatorAPI.Services
{
    public class CatalogApiService : ICatalogApiService
    {
        private readonly HttpClient _httpClient;

        public CatalogApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Product>?> GetProducts()
        {
            var response = await _httpClient.GetAsync("/gateway/Catalog");
            response.EnsureSuccessStatusCode();

            var products = await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();
            return products;
        }

        public async Task<Product?> GetProduct(Guid productId)
        {
            var response = await _httpClient.GetAsync($"/gateway/Catalog/{productId}");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            var product = await response.Content.ReadFromJsonAsync<Product>();
            return product;
        }

        public async Task<IEnumerable<Product>?> GetProductsByCategory(string category)
        {
            var response = await _httpClient.GetAsync($"/gateway/Catalog/GetProductByCategory/{category}");
            response.EnsureSuccessStatusCode();

            var products = await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();
            return products;
        }

        public async Task<Product?> CreateProduct(Product product)
        {
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/gateway/Catalog", content);
            response.EnsureSuccessStatusCode();

            var createdProduct = await response.Content.ReadFromJsonAsync<Product>();
            return createdProduct;
        }

        public async Task<Product?> UpdateProduct(Product product)
        {
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("/gateway/Catalog", content);
            response.EnsureSuccessStatusCode();

            var updatedProduct = await response.Content.ReadFromJsonAsync<Product>();
            return updatedProduct;
        }

        public async Task DeleteProduct(Guid productId)
        {
            var response = await _httpClient.DeleteAsync($"/gateway/Catalog/{productId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
