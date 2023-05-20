using Common.Models;

namespace OrderAPI.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrders(Guid customerId);
        Task<Order> CreateOrder(Order order);
    }
}
