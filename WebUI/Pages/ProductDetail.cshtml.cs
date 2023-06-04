using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebUI.Models;
using WebUI.Services;
using WebUI.Users;

namespace WebUI.Pages
{
    public class ProductDetailModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserProvider _userProvider;

        public ProductDetailModel(IProductService productService, IShoppingCartService shoppingCartService, IUserProvider userProvider)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _shoppingCartService = shoppingCartService ?? throw new ArgumentNullException(nameof(shoppingCartService));
            _userProvider = userProvider ?? throw new ArgumentNullException(nameof(userProvider));
        }

        public ProductWithStock Product { get; set; }

        [BindProperty]
        [Range(1, ushort.MaxValue)]
        public ushort Quantity { get; set; } = 1;

        public async Task<IActionResult> OnGetAsync(Guid productId)
        {
            if (productId == null)
            {
                return NotFound();
            }

            Product = await _productService.GetProduct(productId);
            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(Guid productId)
        {
            var product = await _productService.GetProduct(productId);

            var customerId = _userProvider.GetCustomerId(HttpContext);

            var cart = await _shoppingCartService.AddProductToCart(customerId, product, Quantity);

            return RedirectToPage("Cart");
        }
    }
}