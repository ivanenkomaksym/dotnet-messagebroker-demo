using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Models;
using WebUI.Services;
using WebUI.Users;

namespace WebUI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICatalogService _catalogService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserProvider _userProvider;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ICatalogService catalogService,
                          IShoppingCartService shoppingCartService,
                          IUserProvider userProvider,
                          ILogger<IndexModel> logger)
        {
            _catalogService = catalogService;
            _shoppingCartService = shoppingCartService;
            _userProvider = userProvider;
            _logger = logger;
        }

        public IEnumerable<CatalogModel> ProductList { get; set; } = new List<CatalogModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            ProductList = await _catalogService.GetCatalog();

            var customerId = _userProvider.GetCustomerId(HttpContext);
            var cart = await _shoppingCartService.GetShoppingCart(customerId);

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(Guid productId)
        {
            var product = await _catalogService.GetCatalog(productId);

            var customerId = _userProvider.GetCustomerId(HttpContext);

            var cart = await _shoppingCartService.AddProductToCart(customerId, product);

            return RedirectToPage("Cart");
        }
    }
}