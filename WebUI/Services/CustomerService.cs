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

        public async Task<Customer> GetCustomerById(Guid customerId)
        {
            var response = await _client.GetAsync($"/gateway/Customer/{customerId}");
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<Customer>();
            else
                throw new Exception(response.StatusCode.ToString());
        }

        public async Task<Customer> Authenticate(string email, string password)
        {
            var response = await _client.PostAsync($"/gateway/Customer/Authenticate?email={email}&password={password}", null);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<Customer>();
            else
                throw new Exception(response.StatusCode.ToString());
        }

        public async Task<Customer> CreateCustomer(Customer customer)
        {
            var response = await _client.PostAsJsonAsync($"/gateway/Customer", customer);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<Customer>();
            else
                throw new Exception(response.StatusCode.ToString());
        }

        public async Task<bool> DeleteCustomer(Guid customerId)
        {
            var response = await _client.DeleteAsync($"/gateway/Customer/{customerId}");
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<bool>();
            else
                throw new Exception(response.StatusCode.ToString());
        }
    }
}
