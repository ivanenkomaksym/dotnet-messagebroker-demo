using Microsoft.AspNetCore.Mvc;
using WebUI.Services;

[Route("products")]
public class ProductsController : Controller
{
    private readonly IProductService _productService;

    private readonly object NotImplementedDetailMessage = new
    {
        Message = "Search functionality is not available in this environment. Please use MongoDB Atlas for search capabilities.",
    };

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("autocomplete")]
    public async Task<IActionResult> Autocomplete(string query)
    {
        try
        {
            var results = await _productService.Autocomplete(query);

            if (results == null) return Json(Array.Empty<object>());

            var output = results.Select(p => new {
                id = p.Id,
                name = p.Name,
                imageFile = p.ImageFile
            });

            return Json(output);
        }
        catch (NotImplementedException)
        {
            return StatusCode(501, NotImplementedDetailMessage);
        }
    }
}
