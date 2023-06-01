using Common.Models;
using Common.Models.Payment;

namespace OrderCommon.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderById(Guid orderId);
        Task<IEnumerable<Order>> GetOrdersByCustomerId(Guid customerId);

        Task<Order> CreateOrder(Order order);
        Task<bool> UpdateOrder(Order order);

        Task<bool> UpdatePayment(Guid orderId, PaymentInfo payment);

        Task<bool> Cancel(Guid orderId);

        Task<bool> Collected(Guid orderId);
    }
}
