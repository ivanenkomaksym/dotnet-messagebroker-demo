using Common.Models.Warehouse;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace WarehouseCommon.Data
{
    public class WarehouseContextBase : IWarehouseContext
    {
        public WarehouseContextBase(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            StockItems = database.GetCollection<StockItem>(configuration.GetValue<string>("DatabaseSettings:StockItemsCollectionName"));
            OrderReserves = database.GetCollection<OrderReserve>(configuration.GetValue<string>("DatabaseSettings:ReservesCollectionName"));
        }

        public IMongoCollection<StockItem> StockItems { get; }
        public IMongoCollection<OrderReserve> OrderReserves { get; }
    }
}
