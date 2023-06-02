using Common.Models;

namespace OrderAPI
{
    public interface IOrderService
    {
        public Task CreateOrder(Order order);

        public Task UpdateOrder(Order order);

        public Task CancelOrder(Guid orderId);

        public Task OrderCollected(Guid orderId);
    }
}
