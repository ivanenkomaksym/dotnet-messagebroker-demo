using Common.Models;

namespace WebUI.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrders(Guid customerId);
        Task CreateOrder(Order order);
    }
}
