using CustomerAPI.Entities;
using MongoDB.Driver;

namespace CustomerAPI.Data
{
    public class CustomerContext : ICustomerContext
    {
        public CustomerContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            Customers = database.GetCollection<Customer>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));
        }

        public IMongoCollection<Customer> Customers { get; }
    }
}
