using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Services;
using WebUI.Users;

namespace WebUI.Pages
{
    public class ProductModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserProvider _userProvider;

        public ProductModel(IProductService productService, IShoppingCartService shoppingCartService, IUserProvider userProvider)
        {
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _userProvider = userProvider;
        }

        public IEnumerable<string> CategoryList { get; set; } = new List<string>();

        public IEnumerable<ProductWithStock> ProductList { get; set; } = new List<ProductWithStock>();

        [BindProperty(SupportsGet = true)]
        public string SelectedCategory { get; set; }

        public async Task<IActionResult> OnGetAsync(string categoryName)
        {
            var productList = await _productService.GetProducts();
            CategoryList = productList.Select(p => p.Category).Distinct();

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                ProductList = productList.Where(p => p.Category == categoryName);
                SelectedCategory = categoryName;
            }
            else
            {
                ProductList = productList;
            }

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
