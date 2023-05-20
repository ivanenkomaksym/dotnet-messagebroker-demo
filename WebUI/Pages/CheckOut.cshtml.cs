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

        public Customer Customer { get; set; } = new Customer();

        public ShoppingCartModel ShoppingCart { get; set; } = new ShoppingCartModel();

        [BindProperty]
        public string ErrorMessage { get; set; }

        [BindProperty]
        public bool SaveBillingAddressAndPayment { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);
            if (customerId == Guid.Empty)
                return RedirectToPage("SignIn");

            Customer = await _customerService.GetCustomerById(customerId);

            Order.BillingAddress = Customer.BillingAddress;
            Order.Payment = Customer.Payment;

            ShoppingCart = await _cartService.GetShoppingCart(customerId);

            return Page();
        }

        public async Task<IActionResult> OnPostCheckOutAsync()
        {
            try
            {
                var customerId = _userProvider.GetCustomerId(HttpContext);

                ShoppingCart = await _cartService.GetShoppingCart(customerId);

                if (SaveBillingAddressAndPayment)
                {
                    Customer = await _customerService.GetCustomerById(customerId);

                    Customer.BillingAddress = Order.BillingAddress;
                    Customer.Payment = Order.Payment;

                    await _customerService.UpdateCustomer(Customer);
                }

                Order.CustomerId = customerId;
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
                ErrorMessage = ex.Message;
            }

            return Page();
        }
    }
}
