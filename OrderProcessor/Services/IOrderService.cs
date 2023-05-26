using Common.Models;

namespace OrderProcessor.Services
{
    public interface IOrderService
    {
        Task<Order> GetOrderById(Guid orderId);

        Task<bool> UpdateOrder(Order order);
    }
}
