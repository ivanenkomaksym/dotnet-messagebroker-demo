using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Services;
using WebUI.Users;

namespace WebUI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserProvider _userProvider;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IProductService productService,
                          IShoppingCartService shoppingCartService,
                          IUserProvider userProvider,
                          ILogger<IndexModel> logger)
        {
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _userProvider = userProvider;
            _logger = logger;
        }

        public IEnumerable<ProductWithStock> ProductList { get; set; } = new List<ProductWithStock>();

        public async Task<IActionResult> OnGetAsync()
        {
            ProductList = await _productService.GetProducts();

            var customerId = _userProvider.GetCustomerId(HttpContext);
            var cart = await _shoppingCartService.GetShoppingCart(customerId);

            ProductList = ProductList.OrderBy(productWithStock => productWithStock.AvailableOnStock == 0);

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(Guid productId)
        {
            var product = await _productService.GetProduct(productId);

            var customerId = _userProvider.GetCustomerId(HttpContext);

            var cart = await _shoppingCartService.AddProductToCart(customerId, product);

            return RedirectToPage("Cart");
        }
    }
}