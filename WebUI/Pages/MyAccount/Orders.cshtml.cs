using Common.FeatureManagement;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement;
using WebUI.Services;
using WebUI.Users;

namespace WebUI.Pages.MyAccount
{
    [Authorize]
    public class OrdersModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IUserProvider _userProvider;
        private readonly IFeatureManager _featureManager;

        public OrdersModel(IOrderService orderService, IUserProvider userProvider, IFeatureManager featureManager)
        {
            _orderService = orderService;
            _userProvider = userProvider;
            _featureManager = featureManager;
        }

        public IEnumerable<Order> Orders { get; set; } = new List<Order>();

        [BindProperty]
        public required string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);
            if (customerId == Guid.Empty)
                return RedirectToPage("/Account/Login");

            try
            {
                var orders = await _orderService.GetOrders(customerId);
                ArgumentNullException.ThrowIfNull(orders);
                Orders = orders;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return Page();
        }

        public IActionResult OnPostUpdatePayment(Guid orderId)
        {
            return RedirectToPage("UpdatePayment", new { orderId = orderId, returnUrl = "Orders" });
        }

        public async Task<IActionResult> OnPostCancelAsync(Guid orderId)
        {
            var result = await _orderService.Cancel(orderId);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToPage("Orders");
        }

        public async Task<IActionResult> OnPostConfirmCollectionAsync(Guid orderId)
        {
            var result = await _orderService.Collected(orderId);
            if (!result)
            {
                return NotFound();
            }

            if (await _featureManager.IsEnabledAsync(FeatureFlags.Feedback))
                return RedirectToPage("Feedback", new { orderId = orderId });

            return RedirectToPage("/Index");
        }
    }
}