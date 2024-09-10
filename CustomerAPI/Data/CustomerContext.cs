using Common.Configuration;
using Common.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CustomerAPI.Data
{
    internal class CustomerContext : ICustomerContext
    {
        public CustomerContext(IOptions<DatabaseSettings> databaseSettings)
        {
            var client = new MongoClient(databaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(databaseSettings.Value.DatabaseName);

            Customers = database.GetCollection<Customer>(databaseSettings.Value.CollectionName);
            CustomerContextSeed.SeedData(Customers);
        }

        public IMongoCollection<Customer> Customers { get; }
    }
}
