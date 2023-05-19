using Common.Models;

namespace CustomerAPI
{
    public interface ICustomerService
    {
        public Task CreateCustomer(Customer customer);
    }
}
