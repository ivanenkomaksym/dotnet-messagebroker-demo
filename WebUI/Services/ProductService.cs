using Common.Extensions;
using Common.Models;

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
    }
}
