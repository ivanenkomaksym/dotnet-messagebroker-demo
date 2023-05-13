using Common.Models;
using WebUI.Extensions;

namespace WebUI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly HttpClient _client;

        public CustomerService(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            var response = await _client.GetAsync($"/gateway/Customer");
            return await response.ReadContentAs<IEnumerable<Customer>>();
        }

        public async Task<Customer> GetCustomerByEmail(string email)
        {
            var response = await _client.GetAsync($"/gateway/Customer/{email}");
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<Customer>();
            else
                throw new Exception("Something went wrong when calling api.");
        }

        public async Task<Customer> CreateCustomer(Customer customer)
        {
            var response = await _client.PostAsJsonAsync($"/gateway/Customer", customer);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<Customer>();
            else
                throw new Exception("Something went wrong when calling api.");
        }

        public async Task<bool> DeleteCustomer(Guid customerId)
        {
            var response = await _client.DeleteAsync($"/gateway/Customer/{customerId}");
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<bool>();
            else
                throw new Exception("Something went wrong when calling api.");
        }
    }
}
