using Common.Models.Warehouse;
using MongoDB.Driver;

namespace WarehouseCommon.Data
{
    public interface IWarehouseContextSeed
    {
        public Task SeedData(IMongoCollection<StockItem> stockItemCollection);
    }
}