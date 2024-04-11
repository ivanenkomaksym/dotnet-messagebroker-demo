using Common.Models;

namespace WebUI.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>?> GetCustomers();
        Task<Customer?> GetCustomerById(Guid customerId);
        Task<Customer?> Authenticate(string email, string password);

        Task<Customer?> CreateCustomer(Customer customer);
        Task<bool> UpdateCustomer(Customer customer);
        Task<bool> DeleteCustomer(Guid customerId);
    }
}
