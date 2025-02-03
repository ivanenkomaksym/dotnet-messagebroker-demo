using MongoDB.Driver;
using ShoppingCartAPI.Entities;

namespace ShoppingCartAPI.Data
{
    public interface IShoppingCartContext
    {
        IMongoCollection<ShoppingCart> ShoppingCarts { get; }
    }
}