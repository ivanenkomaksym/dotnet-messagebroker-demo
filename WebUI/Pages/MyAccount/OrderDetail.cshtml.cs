using Common.FeatureManagement;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement;
using WebUI.Services;

namespace WebUI.Pages.MyAccount
{
    [Authorize]
    public class OrderDetailModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IFeatureManager _featureManager;

        public OrderDetailModel(IOrderService orderService, IFeatureManager featureManager)
        {
            _orderService = orderService;
            _featureManager = featureManager;
        }

        public required Order Order { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid orderId)
        {
            var order = await _orderService.GetOrder(orderId);
            ArgumentNullException.ThrowIfNull(order);
            Order = order;
            if (Order == null)
            {
                return NotFound();
            }
            return Page();
        }
        public IActionResult OnPostUpdatePayment(Guid orderId)
        {
            return RedirectToPage("/UpdatePayment", new { orderId, returnUrl = "/Order" });
        }

        public async Task<IActionResult> OnPostCancelAsync(Guid orderId)
        {
            var result = await _orderService.Cancel(orderId);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToPage("/Orders");
        }

        public async Task<IActionResult> OnPostConfirmCollectionAsync(Guid orderId)
        {
            var result = await _orderService.Collected(orderId);
            if (!result)
            {
                return NotFound();
            }

            if (await _featureManager.IsEnabledAsync(FeatureFlags.Feedback))
                return RedirectToPage($"/Orders/{orderId}/Feedback");

            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostReturnOrderAsync(Guid orderId)
        {
            var result = await _orderService.Return(orderId);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToPage("ReturnOrder");
        }
    }
}