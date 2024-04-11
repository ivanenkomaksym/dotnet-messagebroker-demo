using Common.Models;
using Common.SeedData;

namespace WebUI.Services
{
    public class StubCustomerService : ICustomerService
    {
        public Task<Customer?> Authenticate(string email, string password)
        {
            var foundCustomer = Customers.FirstOrDefault(c => c.Email == email);
            if (foundCustomer == null || foundCustomer.Password != password)
                return Task.FromResult<Customer?>(null);

            return Task.FromResult<Customer?>(foundCustomer);
        }

        public Task<Customer?> CreateCustomer(Customer customer)
        {
            return Task.FromResult<Customer?>(customer);
        }

        public Task<bool> DeleteCustomer(Guid customerId)
        {
            return Task.FromResult(true);
        }

        public Task<Customer?> GetCustomerById(Guid customerId)
        {
            var foundCustomer = Customers.FirstOrDefault(c => c.Id == customerId);

            return Task.FromResult(foundCustomer);
        }

        public Task<IEnumerable<Customer>?> GetCustomers()
        {
            return Task.FromResult<IEnumerable<Customer>?>(CustomerSeed.GetPreconfiguredCustomer());
        }

        public Task<bool> UpdateCustomer(Customer customer)
        {
            return Task.FromResult(true);
        }

        private readonly IEnumerable<Customer> Customers = CustomerSeed.GetPreconfiguredCustomer();
    }
}
