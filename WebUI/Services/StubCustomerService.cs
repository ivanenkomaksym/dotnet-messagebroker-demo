using Common.Models;
using Common.SeedData;

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

        private readonly Customer Customer = CustomerSeed.GetPreconfiguredCustomer().ElementAt(0);
    }
}
