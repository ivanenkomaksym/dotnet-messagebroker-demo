using Common.Extensions;
using Common.Models;
using Common.Routing;

namespace WebUI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly HttpClient _client;
        private readonly string _environmentRoutePrefix;

        public CustomerService(HttpClient client, IEnvironmentRouter environmentRouter)
        {
            _client = client;
            _environmentRoutePrefix = environmentRouter.GetCustomerRoute();
        }

        public async Task<IEnumerable<Customer>?> GetCustomers()
        {
            var response = await _client.GetAsync($"{_environmentRoutePrefix}");
            return await response.ReadContentAs<IEnumerable<Customer>>();
        }

        public async Task<Customer?> GetCustomerById(Guid customerId)
        {
            var response = await _client.GetAsync($"{_environmentRoutePrefix}/{customerId}");
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<Customer>();
            else
                return null;
        }

        public async Task<Customer?> Authenticate(string email, string password)
        {
            var response = await _client.PostAsync($"{_environmentRoutePrefix}/Authenticate?email={email}&password={password}", null);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<Customer>();
            else
                throw new Exception(response.StatusCode.ToString());
        }

        public async Task<Customer?> CreateCustomer(Customer customer)
        {
            var response = await _client.PostAsJsonAsync($"{_environmentRoutePrefix}", customer);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<Customer>();
            else
                throw new Exception(response.StatusCode.ToString());
        }

        public async Task<bool> UpdateCustomer(Customer customer)
        {
            var response = await _client.PutAsJsonAsync($"{_environmentRoutePrefix}", customer);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }

            return await response.ReadContentAs<bool>();
        }

        public async Task<bool> DeleteCustomer(Guid customerId)
        {
            var response = await _client.DeleteAsync($"{_environmentRoutePrefix}/{customerId}");
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<bool>();
            else
                throw new Exception(response.StatusCode.ToString());
        }
    }
}