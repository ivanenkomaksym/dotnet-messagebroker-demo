using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Models;
using WebUI.Services;
using WebUI.Users;

namespace WebUI.Pages
{
    public class CheckOutModel : PageModel
    {
        private readonly IShoppingCartService _cartService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IUserProvider _userProvider;

        public CheckOutModel(IShoppingCartService shoppingCartService, IOrderService orderService, ICustomerService customerService, IUserProvider userProvider)
        {
            _cartService = shoppingCartService;
            _orderService = orderService;
            _customerService = customerService;
            _userProvider = userProvider;
        }

        [BindProperty]
        public Order Order { get; set; }

        public Customer Customer { get; set; } = new Customer();

        public ShoppingCartModel ShoppingCart { get; set; } = new ShoppingCartModel();

        public async Task<IActionResult> OnGet()
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);
            Customer = await _customerService.GetCustomerById(customerId);

            ShoppingCart = await _cartService.GetShoppingCart(customerId);

            return Page();
        }

        public async Task<IActionResult> OnPostCheckOutAsync()
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);

            ShoppingCart = await _cartService.GetShoppingCart(customerId);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Order.CustomerId = customerId;
            Order.CustomerName = ShoppingCart.CustomerName;
            Order.TotalPrice = ShoppingCart.TotalPrice;

            foreach (var item in ShoppingCart.Items)
            {
                Order.Items.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity
                });
            }

            await _orderService.CreateOrder(Order);

            await _cartService.DeleteShoppingCart(customerId);

            return RedirectToPage("Confirmation", "OrderSubmitted");
        }
    }
}
