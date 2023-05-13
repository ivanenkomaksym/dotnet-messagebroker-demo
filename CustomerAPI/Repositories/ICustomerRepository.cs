using CustomerAPI.Entities;

namespace CustomerAPI.Repositories
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetCustomers();
        Task<Customer> GetCustomerByEmail(string email);

        Task<Customer> CreateCustomer(Customer customer);
        Task<bool> DeleteCustomer(Guid customerId);
    }
}
