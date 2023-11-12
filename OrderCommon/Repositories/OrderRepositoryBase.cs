using Common.Models;
using Common.Models.Payment;
using MongoDB.Driver;
using OrderCommon.Data;

namespace OrderCommon.Repositories
{
    public class OrderRepositoryBase : IOrderRepository
    {
        protected readonly IOrderContext _context;

        public OrderRepositoryBase(IOrderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public virtual Task<IOrderContext> GetContext()
        {
            return Task.FromResult(_context);
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            var context = await GetContext();
            return await context
                            .Orders
                            .Find(p => true)
                            .ToListAsync();
        }

        public async Task<Order> GetOrderById(Guid orderId)
        {
            var context = await GetContext();

            var matchId = Builders<Order>.Filter.Eq(o => o.Id, orderId);

            return await context
                            .Orders
                            .Find(matchId)
                            .FirstOrDefaultAsync();
        }

        public async Task<Order> CreateOrder(Order order)
        {
            var context = await GetContext();

            order.CreationDateTime = DateTime.Now;
            order.OrderStatus = OrderStatus.New;

            await context.Orders.InsertOneAsync(order);
            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerId(Guid customerId)
        {
            var context = await GetContext();

            var matchId = Builders<Order>.Filter.Eq(o => o.CustomerInfo.CustomerId, customerId);

            return await context
                            .Orders
                            .Find(matchId).ToListAsync();
        }

        public async Task<bool> UpdatePayment(Guid orderId, PaymentInfo payment)
        {
            var order = await GetOrderById(orderId);
            if (order == null)
                return false;

            order.PaymentInfo = payment;
            var result = await UpdateOrder(order);
            return result;
        }

        public async Task<bool> UpdateOrder(Order order)
        {
            var context = await GetContext();
            var updateResult = await context
                                        .Orders
                                        .ReplaceOneAsync(filter: o => o.Id == order.Id, replacement: order);

            return updateResult.IsAcknowledged
                    && updateResult.ModifiedCount > 0;
        }
    }
}
