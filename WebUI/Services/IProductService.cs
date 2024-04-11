using Common.Models;

namespace WebUI.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductWithStock>?> GetProducts();
        Task<IEnumerable<ProductWithStock>?> GetProductsByCategory(string category);
        Task<ProductWithStock?> GetProduct(Guid productId);
        Task<ProductWithStock?> CreateProductWithStock(ProductWithStock productWithStock);
        Task<ProductWithStock?> UpdateProductWithStock(ProductWithStock productWithStock);
        Task DeleteProduct(Guid productId);
    }
}
