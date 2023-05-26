using Common.Models;

namespace OrderAPI.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderById(Guid orderId);
        Task<IEnumerable<Order>> GetOrdersByCustomerId(Guid customerId);
        Task<Order> CreateOrder(Order order);
        Task<bool> UpdateOrder(Order order);
    }
}
