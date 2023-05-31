using Common.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace OrderCommon.Data
{
    public class OrderContext : IOrderContext
    {
        public OrderContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            Orders = database.GetCollection<Order>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));
        }

        public IMongoCollection<Order> Orders { get; }
    }
}
