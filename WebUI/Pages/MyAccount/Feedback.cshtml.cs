using Common.FeatureManagement;
using Common.Models;
using Common.Models.Review;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement.Mvc;
using WebUI.Services;
using WebUI.Users;

namespace WebUI.Pages.MyAccount
{
    [FeatureGate(FeatureFlags.Feedback)]
    public class FeedbackModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IFeedbackService _feedbackService;
        private readonly IUserProvider _userProvider;
        private readonly ICustomerService _customerService;

        public required Order Order { get; set; }

        [BindProperty]
        public required Review Review { get; set; }

        [BindProperty]
        public required bool Anonymous { get; set; }

        public FeedbackModel(IOrderService orderService, IFeedbackService feedbackService, IUserProvider userProvider, ICustomerService customerService)
        {
            _orderService = orderService;
            _feedbackService = feedbackService;
            _userProvider = userProvider;
            _customerService = customerService;
        }

        public async Task<IActionResult> OnGetAsync(Guid orderId)
        {
            var order = await _orderService.GetOrder(orderId);
            ArgumentNullException.ThrowIfNull(order);
            Order = order;

            var customerId = _userProvider.GetCustomerId(HttpContext);
            var customer = await _customerService.GetCustomerById(customerId);
            ArgumentNullException.ThrowIfNull(customer);

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

        public async Task<IActionResult> OnPostAsync(Guid orderId, Guid productId)
        {
            Review.Anonymous = Anonymous;

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(orderId);
            }

            await _feedbackService.PostReview(productId, Review);

            return RedirectToPage(new { orderId = orderId });
        }

        public IActionResult OnPostSkipAsync()
        {
            return RedirectToPage("/Index");
        }

        public async Task<bool> IsFeedbackProvided(Guid orderId, Guid productId)
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);
            var customer = await _customerService.GetCustomerById(customerId);
            ArgumentNullException.ThrowIfNull(customer);

            var reviews = await _feedbackService.GetReviews(productId);
            ArgumentNullException.ThrowIfNull(reviews);
            return reviews.Where(r => r.CustomerInfo.Email == customer.Email).Any();
        }
    }
}