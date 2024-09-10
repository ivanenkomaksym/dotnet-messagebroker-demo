using Common.Configuration;
using Common.Models.Warehouse;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace WarehouseCommon.Data
{
    public class WarehouseContextBase : IWarehouseContext
    {
        public WarehouseContextBase(IConfiguration configuration, IOptions<DatabaseSettings> databaseSettings)
        {
            var client = new MongoClient(databaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(databaseSettings.Value.DatabaseName);

            StockItems = database.GetCollection<StockItem>(configuration.GetValue<string>("DatabaseSettings:StockItemsCollectionName"));
            OrderReserves = database.GetCollection<OrderReserve>(configuration.GetValue<string>("DatabaseSettings:ReservesCollectionName"));
        }

        public IMongoCollection<StockItem> StockItems { get; }
        public IMongoCollection<OrderReserve> OrderReserves { get; }

        public virtual Task InitAsync()
        {
            return Task.CompletedTask;
        }
    }
}
