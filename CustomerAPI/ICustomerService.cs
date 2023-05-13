using CustomerAPI.Entities;

namespace CustomerAPI
{
    public interface ICustomerService
    {
        public Task CreateCustomer(Customer customer);
    }
}
