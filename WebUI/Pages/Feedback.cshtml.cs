using Common.Models;
using Common.Models.Review;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Services;
using WebUI.Users;

namespace WebUI.Pages
{
    public class FeedbackModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IFeedbackService _feedbackService;
        private readonly IUserProvider _userProvider;
        private readonly ICustomerService _customerService;

        public Order Order { get; set; } = new();

        [BindProperty]
        public Review Review { get; set; }

        [BindProperty]
        public bool Anonymous { get; set; }

        public FeedbackModel(IOrderService orderService, IFeedbackService feedbackService, IUserProvider userProvider, ICustomerService customerService)
        {
            _orderService = orderService;
            _feedbackService = feedbackService;
            _userProvider = userProvider;
            _customerService = customerService;
        }

        public async Task<IActionResult> OnGetAsync(Guid orderId)
        {
            Order = await _orderService.GetOrder(orderId);

            var customerId = _userProvider.GetCustomerId(HttpContext);
            var customer = await _customerService.GetCustomerById(customerId);

            Review = new Review
            {
                Id = Guid.NewGuid(),
                Anonymous = false,
                CustomerInfo = new CustomerInfo
                {
                    CustomerId = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email
                },
                CreationDateTime = DateTime.Now,
                ReviewDetails = null
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid productId)
        {
            Review.Anonymous = Anonymous;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _feedbackService.PostReview(productId, Review);

            return Page();
        }

        public async Task<IActionResult> OnPostSkipAsync()
        {
            return RedirectToPage("/Index");
        }
    }
}
