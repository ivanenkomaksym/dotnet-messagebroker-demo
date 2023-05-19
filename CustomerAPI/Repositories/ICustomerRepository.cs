using Common.Models;

namespace CustomerAPI.Repositories
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetCustomers();
        Task<Customer> GetCustomerById(Guid customerId);
        Task<Customer> Authenticate(string email, string password);

        Task<Customer> CreateCustomer(Customer customer);
        Task<bool> DeleteCustomer(Guid customerId);
    }
}
