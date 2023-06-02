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

        public ShoppingCartModel ShoppingCart { get; set; }
        [BindProperty]
        public bool SaveShippingAddressAndPayment { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);
            if (customerId == Guid.Empty)
                return RedirectToPage("/Account/Login", new { returnUrl = "/CheckOut" });

            var customer = await _customerService.GetCustomerById(customerId);
            Order.ShippingAddress = customer.ShippingAddress;
            Order.PaymentInfo = customer.PaymentInfo;

            Order.CustomerInfo = new CustomerInfo
            {
                CustomerId = customerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email
            };

            ShoppingCart = await _cartService.GetShoppingCart(customerId);

            return Page();
        }

        public async Task<IActionResult> OnPostCheckOutAsync()
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);
            var customer = await _customerService.GetCustomerById(customerId);

            if (!ModelState.IsValid)
                return Page();

            try
            {
                ShoppingCart = await _cartService.GetShoppingCart(customerId);

                if (SaveShippingAddressAndPayment)
                {
                    customer.ShippingAddress = Order.ShippingAddress;
                    customer.PaymentInfo = Order.PaymentInfo;

                    await _customerService.UpdateCustomer(customer);
                }

                Order.TotalPrice = ShoppingCart.TotalPrice;

                foreach (var item in ShoppingCart.Items)
                {
                    Order.Items.Add(new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        ProductPrice = item.ProductPrice,
                        Quantity = item.Quantity,
                        ImageFile = item.ImageFile
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
