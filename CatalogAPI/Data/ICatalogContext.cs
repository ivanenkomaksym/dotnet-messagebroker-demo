using Common.Models;
using MongoDB.Driver;

namespace CatalogAPI.Data
{
    public interface ICatalogContext
    {
        IMongoCollection<Product> Products { get; }
    }
}