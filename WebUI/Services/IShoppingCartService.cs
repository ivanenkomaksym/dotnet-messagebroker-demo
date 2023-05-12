using WebUI.Models;

namespace WebUI.Services
{
    public interface IShoppingCartService
    {
        Task<ShoppingCartModel> GetShoppingCart(Guid customerId);

        Task CreateShoppingCart(ShoppingCartModel shoppingCart);
        Task<bool> UpdateShoppingCart(ShoppingCartModel shoppingCart);
        Task<bool> DeleteShoppingCart(Guid customerId);

        Task<bool> Checkout(Guid customerId);
    }
}
