using Common.Models;

namespace WebUI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly HttpClient _client;

        public CustomerService(HttpClient client)
        {
            _client = client;
        }

        public async Task CreateCustomer(Customer customer)
        {
            var response = await _client.PostAsJsonAsync("/gateway/Customer", customer);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Something went wrong when calling api.");
            }
        }
    }
}
