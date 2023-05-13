using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Models;
using WebUI.Services;
using WebUI.Users;

namespace WebUI.Pages
{
    public class CartModel : PageModel
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserProvider _userProvider;

        public CartModel(IShoppingCartService shoppingCartService, IUserProvider userProvider)
        {
            _shoppingCartService = shoppingCartService;
            _userProvider = userProvider;
        }

        public ShoppingCartModel Cart { get; set; } = new ShoppingCartModel();

        public async Task<IActionResult> OnGet()
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);

            Cart = await _shoppingCartService.GetShoppingCart(customerId);

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveToCartAsync(Guid productId)
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);

            var basket = await _shoppingCartService.GetShoppingCart(customerId);

            var item = basket.Items.Single(x => x.ProductId == productId);
            basket.Items.Remove(item);

            var basketUpdated = await _shoppingCartService.UpdateShoppingCart(basket);

            return RedirectToPage();
        }
    }
}
