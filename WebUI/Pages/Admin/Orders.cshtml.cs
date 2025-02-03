using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Services;

namespace WebUI.Pages.Admin
{
    public class OrdersModel : PageModel
    {
        private readonly IOrderService _orderService;

        public OrdersModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [BindProperty]
        public required IEnumerable<Order> Orders { get; set; }


        public async Task OnGetAsync()
        {
            var orders = await _orderService.GetAllOrders();
            ArgumentNullException.ThrowIfNull(orders);
            // Get existing users
            Orders = orders;
        }
    }
}