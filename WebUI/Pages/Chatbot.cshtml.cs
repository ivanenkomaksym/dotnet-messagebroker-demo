using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Services;

namespace WebUI.Pages
{
    public class ChatbotModel : PageModel
    {
        private readonly IProductService _productService;

        public ChatbotModel(IProductService productService)
        {
            _productService = productService;
        }

        // GET /components/chatbot?handler=SemanticSearch&text=your_query
        public async Task<IActionResult> OnGetSemanticSearchAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return BadRequest("Text parameter is required for semantic search.");
            }

            var products = await _productService.FindWithSemanticRelevance(text);

            if (products == null || !products.Any())
            {
                return NotFound(); // Or return an empty array if preferred
            }

            // Return relevant product details
            var result = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Category,
                p.Author,
                p.Summary,
                p.ImageFile,
                p.Price
            });

            return new JsonResult(result);
        }
    }
}
