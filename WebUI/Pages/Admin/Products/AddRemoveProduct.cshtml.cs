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
        public ProductWithStock NewProduct { get; set; }

        [BindProperty]
        public IEnumerable<ProductWithStock> Products { get; private set; }

        public AddRemoveProductModel(IProductService productService)
        {
            _productService = productService;
        }

        public async Task OnGetAsync()
        {
            // Get existing products
            Products = await _productService.GetProducts();

            // Initialize the NewProduct with a new Guid for the Id property
            NewProduct = new ProductWithStock
            {
                Id = Guid.NewGuid()
            };
        }

        public async Task<IActionResult> OnPostAddProduct()
        {
            if (!ModelState.IsValid)
            {
                // If the model state is invalid, refresh the page with the existing input fields.
                Products = await _productService.GetProducts();
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

        // Method to fetch all products with stock information
        public async Task<IEnumerable<ProductWithStock>> GetProductsAsync()
        {
            return await _productService.GetProducts();
        }
    }
}
