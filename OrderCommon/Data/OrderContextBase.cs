using Common.Configuration;
using Common.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace OrderCommon.Data
{
    public class OrderContextBase : IOrderContext
    {
        public OrderContextBase(IOptions<DatabaseSettings> databaseSettings)
        {
            var client = new MongoClient(databaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(databaseSettings.Value.DatabaseName);

            Orders = database.GetCollection<Order>(databaseSettings.Value.CollectionName);
        }

        public IMongoCollection<Order> Orders { get; }

        public virtual Task InitAsync()
        {
            return Task.CompletedTask;
        }
    }
}
