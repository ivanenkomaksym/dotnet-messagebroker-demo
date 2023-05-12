using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Models;
using WebUI.Services;

namespace WebUI.Pages
{
    public class ProductDetailModel : PageModel
    {
        private readonly ICatalogService _catalogService;
        private readonly IShoppingCartService _shoppingCartService;

        public ProductDetailModel(ICatalogService catalogService, IShoppingCartService shoppingCartService)
        {
            _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            _shoppingCartService = shoppingCartService; 
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

            var cart = await _shoppingCartService.GetShoppingCart(CustomerId);

            cart.Items.Add(new ShoppingCartItemModel
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                ProductName = product.Name,
                ProductPrice = product.Price,
                Quantity = Quantity
            });

            var basketUpdated = await _shoppingCartService.UpdateShoppingCart(cart);

            return RedirectToPage("Cart");
        }
    }
}