using Common.Models;

namespace WebUI.Services
{
    public interface ICustomerService
    {
        public Task CreateCustomer(Customer customer);
    }
}
