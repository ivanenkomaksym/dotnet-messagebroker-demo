using Common.Models;

namespace OrderCommon.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderById(Guid orderId);
        Task<IEnumerable<Order>> GetOrdersByCustomerId(Guid customerId);
        Task<Order> CreateOrder(Order order);
        Task<bool> UpdateOrder(Order order);
    }
}
