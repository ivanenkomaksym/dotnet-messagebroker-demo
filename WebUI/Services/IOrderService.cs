using Common.Models;

namespace WebUI.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrders(Guid customerId);

        Task<Order> GetOrder(Guid orderId);
        Task CreateOrder(Order order);

        Task<bool> UpdateOrder(Order order);
    }
}
