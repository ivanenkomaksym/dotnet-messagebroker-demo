using Common.Models;
using Common.Models.Payment;

namespace WebUI.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrders(Guid customerId);

        Task<Order> GetOrder(Guid orderId);
        Task CreateOrder(Order order);

        Task<bool> UpdatePayment(Guid orderId, PaymentInfo payment);

        Task<bool> Cancel(Guid orderId);

        Task<bool> Collected(Guid orderId);

        Task<bool> Return(Guid orderId);
    }
}
