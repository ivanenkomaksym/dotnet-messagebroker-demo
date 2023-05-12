using ShoppingCartAPI.Entities;

namespace ShoppingCartAPI.Repositories
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCart> GetShoppingCart(Guid customerId);

        Task CreateShoppingCart(ShoppingCart shoppingCart);
        Task<bool> UpdateShoppingCart(ShoppingCart shoppingCart);
        Task<bool> DeleteShoppingCart(Guid customerId);

        Task<bool> Checkout(Guid customerId);
    }
}
