using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Services;

namespace WebUI.Pages.Admin.Products
{
    public class AddRemoveProductModel : PageModel
    {
        private readonly IProductService _productService;

        [BindProperty]
        public required ProductWithStock NewProduct { get; set; }

        [BindProperty]
        public required IEnumerable<ProductWithStock> Products { get; set; }

        public AddRemoveProductModel(IProductService productService)
        {
            _productService = productService;
        }

        public async Task OnGetAsync()
        {
            var products = await _productService.GetProducts();
            ArgumentNullException.ThrowIfNull(products);
            // Get existing products
            Products = products;

            // Initialize the NewProduct with a new Guid for the Id property
            NewProduct = new ProductWithStock
            {
                Id = Guid.NewGuid(),
                Name = string.Empty,
                Category = string.Empty,
                Summary = string.Empty,
                ImageFile = string.Empty
            };
        }

        public async Task<IActionResult> OnPostAddProduct()
        {
            if (!ModelState.IsValid)
            {
                var products = await _productService.GetProducts();
                ArgumentNullException.ThrowIfNull(products);
                // If the model state is invalid, refresh the page with the existing input fields.
                Products = products;
                return Page();
            }

            await _productService.CreateProductWithStock(NewProduct);

            // Refresh the page with the updated product list.
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteProduct(Guid productId)
        {
            await _productService.DeleteProduct(productId);

            // Refresh the page with the updated product list.
            return RedirectToPage();
        }
    }
}
