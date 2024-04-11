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

        [BindProperty]
        public required ShoppingCartModel Cart { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);
            var cart = await _shoppingCartService.GetShoppingCart(customerId);
            ArgumentNullException.ThrowIfNull(cart);
            Cart = cart;

            return Page();
        }

        public async Task<IActionResult> OnPostIncreaseQuantity(Guid itemId, ushort quantity)
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);

            var cart = await _shoppingCartService.GetShoppingCart(customerId);
            ArgumentNullException.ThrowIfNull(cart);

            Cart = cart;

            // Find the item in the shopping cart
            var item = Cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                // Increase the quantity
                item.Quantity = quantity;

                // Update the total price of the shopping cart
                Cart.TotalPrice = CalculateTotalPrice();
            }

            var basketUpdated = await _shoppingCartService.UpdateShoppingCart(Cart);

            return Page();
        }

        private decimal CalculateTotalPrice()
        {        
            // Calculate the total price based on the items in the shopping cart
            return Cart.Items.Sum(item => item.ProductPrice * item.Quantity);
        }

        public async Task<IActionResult> OnPostRemoveToCartAsync(Guid productId)
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);

            var cart = await _shoppingCartService.GetShoppingCart(customerId);
            ArgumentNullException.ThrowIfNull(cart);

            var item = cart.Items.Single(x => x.ProductId == productId);
            cart.Items.Remove(item);

            var basketUpdated = await _shoppingCartService.UpdateShoppingCart(cart);

            return RedirectToPage();
        }
    }
}
