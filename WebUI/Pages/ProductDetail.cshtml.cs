using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.ContentModel;
using WebUI.Models;
using WebUI.Services;
using WebUI.Users;

namespace WebUI.Pages
{
    public class ProductDetailModel : PageModel
    {
        private readonly ICatalogService _catalogService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserProvider _userProvider;

        public ProductDetailModel(ICatalogService catalogService, IShoppingCartService shoppingCartService, IUserProvider userProvider)
        {
            _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            _shoppingCartService = shoppingCartService ?? throw new ArgumentNullException(nameof(shoppingCartService));
            _userProvider = userProvider ?? throw new ArgumentNullException(nameof(userProvider));
        }

        public CatalogModel Product { get; set; }

        [BindProperty]
        public string Color { get; set; }

        [BindProperty]
        public ushort Quantity { get; set; }

        [TempData]
        public Guid CustomerId { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid productId)
        {
            if (productId == null)
            {
                return NotFound();
            }

            Product = await _catalogService.GetCatalog(productId);
            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(Guid productId)
        {
            var product = await _catalogService.GetCatalog(productId);

            var customerId = _userProvider.GetCustomerId(HttpContext);

            var cart = await _shoppingCartService.GetShoppingCart(customerId);

            var items = cart.Items.Where(x => x.ProductId == productId);
            if (items.Any())
            {
                items.First().Quantity++;
            }
            else
            {
                cart.Items.Add(new ShoppingCartItemModel
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    ProductName = product.Name,
                    ProductPrice = product.Price,
                    Quantity = Quantity
                });
            }

            var basketUpdated = await _shoppingCartService.UpdateShoppingCart(cart);

            return RedirectToPage("Cart");
        }
    }
}