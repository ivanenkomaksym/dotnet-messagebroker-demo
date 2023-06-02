using Common.Models;
using Common.Models.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Services;
using WebUI.Users;

namespace WebUI.Pages
{
    public class UpdatePaymentModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IUserProvider _userProvider;

        public UpdatePaymentModel(IOrderService orderService, IUserProvider userProvider)
        {
            _orderService = orderService;
            _userProvider = userProvider;
        }

        [BindProperty]
        public Guid OrderId { get; set; }

        [BindProperty]
        public PaymentInfo PaymentInfo { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid orderId)
        {
            OrderId = orderId;
            var order = await _orderService.GetOrder(orderId);
            if (order == null)
            {
                return NotFound();
            }

            PaymentInfo = order.PaymentInfo;
            return Page();
        }

        public async Task<IActionResult> OnPostUpdatePaymentAsync(Guid orderId)
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                await _orderService.UpdatePayment(orderId, PaymentInfo);
                return RedirectToPage("OrderDetail", new { orderId = orderId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(Order.Id), "Problem updating order.");
            }

            return Page();
        }
    }
}
