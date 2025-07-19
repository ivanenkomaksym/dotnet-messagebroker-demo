using Common.Models;

namespace WebUIAggregatorAPI.Services
{
    public interface ICatalogApiService
    {
        public Task<IEnumerable<Product>?> GetProducts();

        public Task<Product?> GetProduct(Guid productId);

        public Task<IEnumerable<Product>?> GetProductsByCategory(string category);

        public Task<Product?> CreateProduct(Product product);

        public Task<Product?> UpdateProduct(Product product);

        public Task DeleteProduct(Guid productId);

        public Task<IEnumerable<Product>?> Autocomplete(string query);

        public Task<IEnumerable<Product>?> FindWithSemanticRelevance(string text);
    }
}