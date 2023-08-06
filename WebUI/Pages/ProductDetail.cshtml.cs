using Common.Models;
using Common.Models.Review;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebUI.Services;
using WebUI.Users;

namespace WebUI.Pages
{
    public class ProductDetailModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserProvider _userProvider;
        private readonly IFeedbackService _feedbackService;

        public ProductDetailModel(IProductService productService,
                                  IShoppingCartService shoppingCartService,
                                  IUserProvider userProvider,
                                  IFeedbackService feedbackService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _shoppingCartService = shoppingCartService ?? throw new ArgumentNullException(nameof(shoppingCartService));
            _userProvider = userProvider ?? throw new ArgumentNullException(nameof(userProvider));
            _feedbackService = feedbackService;
        }

        public ProductWithStock Product { get; set; }

        public IEnumerable<Review> Reviews { get; set; }

        public decimal StarRatio(int star)
        {
            if (Reviews == null || !Reviews.Any())
            {
                return 0;
            }

            int starCount = Reviews.Count(review => review.Rating == star);
            return (decimal)starCount / Reviews.Count();
        }

        public double AverageRating { get; set; }

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

            Reviews = await _feedbackService.GetReviews(productId);
            var ratings = Reviews.Select(r => r.Rating);
            if (ratings.Any())
                AverageRating = ratings.Average();

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(Guid productId)
        {
            var product = await _productService.GetProduct(productId);

            var customerId = _userProvider.GetCustomerId(HttpContext);

            var cart = await _shoppingCartService.AddProductToCart(customerId, product, Quantity);

            return RedirectToPage("Cart");
        }

        public string ObfuscateString(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= 2)
            {
                return input;
            }

            char firstChar = input[0];
            char lastChar = input[input.Length - 1];
            string middleAsterisks = new string('*', input.Length - 2);

            return firstChar + middleAsterisks + lastChar;
        }
    }
}