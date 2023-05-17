using WebUI.Models;

namespace WebUI.Services
{
    public interface IShoppingCartService
    {
        Task<ShoppingCartModel> GetShoppingCart(Guid customerId);

        Task CreateShoppingCart(ShoppingCartModel shoppingCart);
        Task<ShoppingCartModel> UpdateShoppingCart(ShoppingCartModel shoppingCart);
        Task<bool> DeleteShoppingCart(Guid customerId);

        Task<ShoppingCartModel> AddProductToCart(Guid customerId, CatalogModel product);

        Task<bool> Checkout(Guid customerId);
    }
}
