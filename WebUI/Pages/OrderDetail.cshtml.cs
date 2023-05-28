using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Services;
using WebUI.Users;

namespace WebUI.Pages
{
    [Authorize]
    public class OrderDetailModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IUserProvider _userProvider;

        public OrderDetailModel(IOrderService orderService, IUserProvider userProvider)
        {
            _orderService = orderService;
            _userProvider = userProvider;
        }

        [BindProperty]
        public Order Order { get; set; }

        public async Task<IActionResult> OnGet(Guid orderId)
        {
            if (orderId == null)
            {
                return NotFound();
            }

            Order = await _orderService.GetOrder(orderId);
            if (Order == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateOrderAsync(Order order)
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                _orderService.UpdateOrder(order);
                return RedirectToPage("Order");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(Order.Id), "Problem updating order.");
            }

            return Page();
        }
    }
}
