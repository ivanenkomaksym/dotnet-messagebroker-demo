using WebUI.Models;

namespace WebUI.Services
{
    public interface ICatalogService
    {
        Task<IEnumerable<CatalogModel>> GetCatalog();
        Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category);
        Task<CatalogModel> GetCatalog(Guid productId);
        Task<CatalogModel> CreateCatalog(CatalogModel model);
    }
}
