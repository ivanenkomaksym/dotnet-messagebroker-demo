using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Services;
using WebUI.Users;

namespace WebUI.Pages
{
    [Authorize]
    public class OrderModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IUserProvider _userProvider;

        public OrderModel(IOrderService orderService, IUserProvider userProvider)
        {
            _orderService = orderService;
            _userProvider = userProvider;
        }

        public IEnumerable<Order> Orders { get; set; } = new List<Order>();

        [BindProperty]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);
            if (customerId == Guid.Empty)
                return RedirectToPage("/Account/Login");

            try
            {
                Orders = await _orderService.GetOrders(customerId);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return Page();
        }

        public IActionResult OnPostUpdatePayment(Guid orderId)
        {
            return RedirectToPage("/UpdatePayment", new { orderId = orderId });
        }
    }
}
