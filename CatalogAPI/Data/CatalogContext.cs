using Common.Configuration;
using Common.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CatalogAPI.Data
{
    public class CatalogContext: ICatalogContext
    {
        public CatalogContext(IOptions<DatabaseSettings> databaseSettings)
        {
            var client = new MongoClient(databaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(databaseSettings.Value.DatabaseName);

            Products = database.GetCollection<Product>(databaseSettings.Value.CollectionName);
            CatalogContextSeed.SeedData(Products);
        }

        public IMongoCollection<Product> Products { get; }
    }
}