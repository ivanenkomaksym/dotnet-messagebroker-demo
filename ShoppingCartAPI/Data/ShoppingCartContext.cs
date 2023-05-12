using MongoDB.Driver;
using ShoppingCartAPI.Entities;

namespace ShoppingCartAPI.Data
{
    public class ShoppingCartContext : IShoppingCartContext
    {
        public ShoppingCartContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            ShoppingCarts = database.GetCollection<ShoppingCart>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));
        }

        public IMongoCollection<ShoppingCart> ShoppingCarts { get; set; }
    }
}
