using System.Net;
using System.Text;
using System.Text.Json;
using Common.Models;
using Common.Routing;

namespace WebUIAggregatorAPI.Services
{
    public class CatalogApiService : ICatalogApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _environmentRoutePrefix;

        public CatalogApiService(HttpClient httpClient, IEnvironmentRouter environmentRouter)
        {
            _httpClient = httpClient;
            _environmentRoutePrefix = environmentRouter.GetCatalogRoute();
        }

        public async Task<IEnumerable<Product>?> GetProducts()
        {
            var response = await _httpClient.GetAsync($"{_environmentRoutePrefix}");
            response.EnsureSuccessStatusCode();

            var products = await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();
            return products;
        }

        public async Task<Product?> GetProduct(Guid productId)
        {
            var response = await _httpClient.GetAsync($"{_environmentRoutePrefix}/{productId}");
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
            var response = await _httpClient.GetAsync($"{_environmentRoutePrefix}/GetProductByCategory/{category}");
            response.EnsureSuccessStatusCode();

            var products = await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();
            return products;
        }

        public async Task<Product?> CreateProduct(Product product)
        {
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_environmentRoutePrefix}", content);
            response.EnsureSuccessStatusCode();

            var createdProduct = await response.Content.ReadFromJsonAsync<Product>();
            return createdProduct;
        }

        public async Task<Product?> UpdateProduct(Product product)
        {
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_environmentRoutePrefix}", content);
            response.EnsureSuccessStatusCode();

            var updatedProduct = await response.Content.ReadFromJsonAsync<Product>();
            return updatedProduct;
        }

        public async Task DeleteProduct(Guid productId)
        {
            var response = await _httpClient.DeleteAsync($"{_environmentRoutePrefix}/{productId}");
            response.EnsureSuccessStatusCode();
        }
    }
}