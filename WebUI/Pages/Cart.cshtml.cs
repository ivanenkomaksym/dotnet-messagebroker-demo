using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Models;
using WebUI.Services;

namespace WebUI.Pages
{
    public class CartModel : PageModel
    {
        private readonly IShoppingCartService _shoppingCartService;

        public CartModel(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        public ShoppingCartModel Cart { get; set; } = new ShoppingCartModel();

        [TempData]
        public Guid CustomerId { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Cart = await _shoppingCartService.GetShoppingCart(CustomerId);

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveToCartAsync(Guid productId)
        {
            var basket = await _shoppingCartService.GetShoppingCart(CustomerId);

            var item = basket.Items.Single(x => x.ProductId == productId);
            basket.Items.Remove(item);

            var basketUpdated = await _shoppingCartService.UpdateShoppingCart(basket);

            return RedirectToPage();
        }
    }
}
