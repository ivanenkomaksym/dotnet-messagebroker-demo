using Common.Models.Warehouse;
using MongoDB.Driver;

namespace WarehouseAPI.Data
{
    public interface IWarehouseContextSeed
    {
        public Task SeedData(IMongoCollection<StockItem> stockItemCollection);
    }
}