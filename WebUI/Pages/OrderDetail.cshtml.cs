using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Services;

namespace WebUI.Pages
{
    [Authorize]
    public class OrderDetailModel : PageModel
    {
        private readonly IOrderService _orderService;

        public OrderDetailModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public Order Order { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid orderId)
        {
            Order = await _orderService.GetOrder(orderId);
            if (Order == null)
            {
                return NotFound();
            }
            return Page();
        }
        public IActionResult OnPostUpdatePayment(Guid orderId)
        {
            return RedirectToPage("/UpdatePayment", new { orderId = orderId, returnUrl = "/Order" });
        }

        public async Task<IActionResult> OnPostCancelAsync(Guid orderId)
        {
            var result = await _orderService.Cancel(orderId);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToPage("/Order");
        }

        public async Task<IActionResult> OnPostConfirmCollectionAsync(Guid orderId)
        {
            var result = await _orderService.Collected(orderId);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToPage($"/Order/{orderId}/Feedback");
        }

        public async Task<IActionResult> OnPostReturnOrderAsync(Guid orderId)
        {
            var result = await _orderService.Return(orderId);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToPage("/ReturnOrder");
        }
    }
}
