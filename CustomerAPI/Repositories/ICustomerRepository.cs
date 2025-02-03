using Common.Models;

namespace CustomerAPI.Repositories
{
    /// <summary>
    /// Used to access customer's repository.
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Get all customers.
        /// </summary>
        /// <returns>List of customers.</returns>
        Task<IEnumerable<Customer>> GetCustomers();

        /// <summary>
        /// Get customer by id.
        /// </summary>
        /// <param name="customerId">Customer id.</param>
        /// <returns>Customer if found.</returns>
        Task<Customer> GetCustomerById(Guid customerId);

        /// <summary>
        /// Authenticates user by email and password.
        /// </summary>
        /// <param name="email">Email.</param>
        /// <param name="password">Password.</param>
        /// <returns>Customer if authenticated.</returns>
        Task<Customer> Authenticate(string email, string password);

        /// <summary>
        /// Create new customer.
        /// </summary>
        /// <param name="customer">Customer.</param>
        /// <returns>True if created.</returns>
        Task<Customer?> CreateCustomer(Customer customer);

        /// <summary>
        /// Update existing customer.
        /// </summary>
        /// <param name="customer">Customer to be updated/</param>
        /// <returns>True if updated.</returns>
        Task<bool> UpdateCustomer(Customer customer);

        /// <summary>
        /// Delete customer by id.
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <returns>NoContent if deleted.</returns>
        Task<bool> DeleteCustomer(Guid customerId);
    }
}