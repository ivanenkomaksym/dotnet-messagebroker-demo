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
        public IEnumerable<Order> Orders { get; private set; }


        public async Task OnGetAsync()
        {
            // Get existing users
            Orders = await _orderService.GetAllOrders();
        }
    }
}
