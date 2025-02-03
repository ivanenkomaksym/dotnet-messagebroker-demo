using Common.Models;
using Common.Models.Payment;
using OrderCommon.Data;

namespace OrderCommon.Repositories
{
    public interface IOrderRepository
    {
        Task<IOrderContext> GetContext();

        Task<IEnumerable<Order>> GetAllOrders();
        Task<Order> GetOrderById(Guid orderId);
        Task<IEnumerable<Order>> GetOrdersByCustomerId(Guid customerId);

        Task<Order> CreateOrder(Order order);
        Task<bool> UpdateOrder(Order order);

        Task<bool> UpdatePayment(Guid orderId, PaymentInfo payment);
    }
}