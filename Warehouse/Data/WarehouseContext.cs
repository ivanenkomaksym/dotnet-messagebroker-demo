using Common.Models.Warehouse;
using MongoDB.Driver;

namespace Warehouse.Data
{
    internal class WarehouseContext : IWarehouseContext
    {
        public WarehouseContext(IWarehouseContextSeed warehouseContextSeed, IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            StockItems = database.GetCollection<StockItem>(configuration.GetValue<string>("DatabaseSettings:StockItemsCollectionName"));
            OrderReserves = database.GetCollection<OrderReserve>(configuration.GetValue<string>("DatabaseSettings:ReservesCollectionName"));

            warehouseContextSeed.SeedData(StockItems);
        }

        public IMongoCollection<StockItem> StockItems { get; }
        public IMongoCollection<OrderReserve> OrderReserves { get; }
    }
}
