using Common.Models.Warehouse;
using MongoDB.Driver;

namespace Warehouse.Data
{
    public interface IWarehouseContextSeed
    {
        public Task SeedData(IMongoCollection<StockItem> stockItemCollection);
    }
}