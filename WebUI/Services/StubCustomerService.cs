using Common.Models;
using Common.Models.Payment;

namespace WebUI.Services
{
    public class StubCustomerService : ICustomerService
    {
        public Task<Customer> Authenticate(string email, string password)
        {
            return Task.FromResult(Customer);
        }

        public Task<Customer> CreateCustomer(Customer customer)
        {
            return Task.FromResult(Customer);
        }

        public Task<bool> DeleteCustomer(Guid customerId)
        {
            return Task.FromResult(true);
        }

        public Task<Customer> GetCustomerById(Guid customerId)
        {
            return Task.FromResult(Customer);
        }

        public Task<IEnumerable<Customer>> GetCustomers()
        {
            IEnumerable<Customer> customers = new List<Customer> { Customer };
            return Task.FromResult(customers);
        }

        public Task<bool> UpdateCustomer(Customer customer)
        {
            return Task.FromResult(true);
        }

        private readonly Customer Customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Alice",
            LastName = "Liddell",
            Email = "alice@gmail.com",
            Password = "alice",
            PaymentInfo = new PaymentInfo
            {
                CardName = "Alice Liddell",
                CardNumber = "1234 1234 1234 1234",
                CVV = "123",
                Expiration = "01/30",
                PaymentMethod = PaymentMethod.CreditCard_AlwaysExpire
            },
            ShippingAddress = new Address
            {
                FirstName = "Alice",
                LastName = "Liddell",
                Email = "alice@gmail.com",
                Country = "England",
                AddressLine = "London",
                ZipCode = "12345"
            },
            CreationDateTime = DateTime.Now,
        };
    }
}
