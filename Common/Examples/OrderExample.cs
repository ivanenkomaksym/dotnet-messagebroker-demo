using Common.Models;
using Common.Models.Payment;
using Swashbuckle.AspNetCore.Filters;

namespace Common.Examples
{
    public class OrderExample : IExamplesProvider<Order>
    {
        public Order GetExamples()
        {
            return new Order
            {
                Id = Guid.NewGuid(),
                CustomerInfo = new()
                {
                    CustomerId = Guid.NewGuid(),
                    FirstName = "Alice",
                    LastName = "Liddell",
                    Email = "alice@gmail.com"
                },
                Items = new[]
                {
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.NewGuid(),
                        ProductName = "Product 1",
                        Quantity = 1,
                        ProductPrice = 10.0m
                    }
                },
                TotalPrice = 10.0m,
                ShippingAddress = new Address
                {
                    FirstName = "Alice",
                    LastName = "Liddell",
                    Email = "alice@gmail.com",
                    AddressLine = "London",
                    Country = "GB",
                    ZipCode = "10000"
                },
                PaymentInfo = new PaymentInfo
                {
                    CardName = "Alice Liddell",
                    CardNumber = "1234 5678 9101 1121 3141",
                    Expiration = "01/28",
                    CVV = "123",
                    PaymentMethod = 0
                }
            };
        }
    }
}
