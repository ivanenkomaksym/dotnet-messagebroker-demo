using Common.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ShoppingCartAPI.Entities;

namespace ShoppingCartAPI.Data
{
    public class ShoppingCartContext : IShoppingCartContext
    {
        public ShoppingCartContext(IOptions<DatabaseSettings> databaseSettings)
        {
            var client = new MongoClient(databaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(databaseSettings.Value.DatabaseName);

            ShoppingCarts = database.GetCollection<ShoppingCart>(databaseSettings.Value.CollectionName);
        }

        public IMongoCollection<ShoppingCart> ShoppingCarts { get; set; }
    }
}
