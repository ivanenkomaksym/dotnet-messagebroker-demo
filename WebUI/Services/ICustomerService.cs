using Common.Models;

namespace WebUI.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetCustomers();
        Task<Customer> GetCustomerByEmail(string email);

        Task<Customer> CreateCustomer(Customer customer);
        Task<bool> DeleteCustomer(Guid customerId);
    }
}
