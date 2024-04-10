using Common.Models;

namespace OrderAPI.Messaging
{
    /// <summary>
    /// Used for publishing messages about changes in order's repository.
    /// </summary>
    public interface IOrderPublisher
    {
        /// <summary>
        /// Create order.
        /// </summary>
        /// <param name="order">New order</param>
        /// <returns>Task indicating completion.</returns>
        public Task CreateOrder(Order order);

        /// <summary>
        /// Update order.
        /// </summary>
        /// <param name="order">Updated order.</param>
        /// <returns>Task indicating completion.</returns>
        public Task UpdateOrder(Order order);

        /// <summary>
        /// Cancel order.
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>Task indicating completion.</returns>
        /// <returns></returns>
        public Task CancelOrder(Guid orderId);

        /// <summary>
        /// Confirm order collection.
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>Task indicating completion.</returns>
        public Task OrderCollected(Guid orderId);

        /// <summary>
        /// Return order.
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>Task indicating completion.</returns>
        public Task ReturnOrder(Guid orderId);
    }
}
