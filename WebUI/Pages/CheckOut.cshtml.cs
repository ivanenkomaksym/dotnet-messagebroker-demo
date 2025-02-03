using System.ComponentModel.DataAnnotations;
using Common.Models;
using Common.Models.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Data;
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
        public required Order Order { get; set; } = new Order
        {
            CustomerInfo = new CustomerInfo { FirstName = string.Empty, LastName = string.Empty, Email = string.Empty },
            PaymentInfo = new PaymentInfo { CardName = string.Empty, CardNumber = string.Empty, Expiration = string.Empty, CVV = string.Empty },
            ShippingAddress = new Address { FirstName = string.Empty, LastName = string.Empty, Email = string.Empty, AddressLine = string.Empty, Country = string.Empty, ZipCode = string.Empty }
        };

        public required ShoppingCartModel ShoppingCart { get; set; }

        [BindProperty]
        public bool SignUp { get; set; }

        [DataType(DataType.Password)]
        public required string CustomerPassword { get; set; }

        [BindProperty]
        public bool SaveShippingAddressAndPayment { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);

            var customer = await _customerService.GetCustomerById(customerId);
            if (customer != null)
            {
                Order.ShippingAddress = customer.ShippingAddress;
                Order.PaymentInfo = customer.PaymentInfo;

                Order.CustomerInfo = new CustomerInfo
                {
                    CustomerId = customerId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email
                };
            }
            else
            {
                Order.CustomerInfo = new CustomerInfo
                {
                    CustomerId = customerId,
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    Email = string.Empty
                };
            }

            var cart = await _cartService.GetShoppingCart(customerId);
            ArgumentNullException.ThrowIfNull(cart);

            ShoppingCart = cart;

            return Page();
        }

        public async Task<IActionResult> OnPostCheckOutAsync()
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);

            if (!ModelState.IsValid)
                return await OnGet();

            try
            {
                var cart = await _cartService.GetShoppingCart(customerId);
                ArgumentNullException.ThrowIfNull(cart);
                ShoppingCart = cart;

                if (SignUp)
                {
                    ArgumentNullException.ThrowIfNull(Order);
                    ArgumentNullException.ThrowIfNull(Order.CustomerInfo);

                    var customer = new Customer
                    {
                        Id = customerId,
                        Email = Order.CustomerInfo.Email,
                        FirstName = Order.CustomerInfo.FirstName,
                        LastName = Order.CustomerInfo.LastName,
                        Password = CustomerPassword,
                        ShippingAddress = Order.ShippingAddress,
                        PaymentInfo = Order.PaymentInfo
                    };

                    try
                    {
                        await _customerService.CreateCustomer(customer);

                        var user = new ApplicationUser
                        {
                            CustomerId = customer.Id,
                            Email = customer.Email,
                            FullName = $"{customer.FirstName} {customer.LastName}"
                        };

                        await _userProvider.SignInAsync(HttpContext, user);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError(nameof(Customer.Email), "User with this email already exists.");
                        return await OnGet();
                    }
                }

                if (SaveShippingAddressAndPayment)
                {
                    var customer = await _customerService.GetCustomerById(customerId);
                    ArgumentNullException.ThrowIfNull(customer);

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
            catch (Exception)
            {
                ModelState.AddModelError(nameof(Order.Id), "Problem creating order.");
                return await OnGet();
            }
        }
    }
}