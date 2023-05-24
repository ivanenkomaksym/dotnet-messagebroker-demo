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
        public Order Order { get; set; } = new Order();

        public ShoppingCartModel ShoppingCart { get; set; } = new ShoppingCartModel();
        [BindProperty]
        public bool SaveShippingAddressAndPayment { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);
            if (customerId == Guid.Empty)
                return RedirectToPage("SignIn");

            var customer = await _customerService.GetCustomerById(customerId);

            Order.ShippingAddress = customer.ShippingAddress;
            Order.Payment = customer.Payment;

            return Page();
        }

        public async Task<IActionResult> OnPostCheckOutAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                var customerId = _userProvider.GetCustomerId(HttpContext);

                ShoppingCart = await _cartService.GetShoppingCart(customerId);

                var customer = await _customerService.GetCustomerById(customerId);
                if (SaveShippingAddressAndPayment)
                {
                    customer.ShippingAddress = Order.ShippingAddress;
                    customer.Payment = Order.Payment;

                    await _customerService.UpdateCustomer(customer);
                }

                Order.CustomerInfo = new()
                {
                    CustomerId = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email
                };
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
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(Order.Id), "Problem creating order.");
            }

            return Page();
        }
    }
}
