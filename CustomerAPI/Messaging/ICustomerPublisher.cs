using Common.Models;

namespace CustomerAPI.Messaging
{
    /// <summary>
    /// Used for publishing messages about changes in customer's repository.
    /// </summary>
    public interface ICustomerPublisher
    {
        /// <summary>
        /// Customer created message.
        /// </summary>
        /// <param name="customer">Customer.</param>
        /// <returns>Task indicating completion.</returns>
        public Task CreateCustomer(Customer customer);
    }
}