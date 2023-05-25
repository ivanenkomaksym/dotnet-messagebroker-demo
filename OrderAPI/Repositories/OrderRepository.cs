using Common.Models;
using MongoDB.Driver;
using OrderAPI.Data;

namespace OrderAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IOrderContext _context;

        public OrderRepository(IOrderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Order> GetOrderById(Guid orderId)
        {
            var matchId = Builders<Order>.Filter.Eq(o => o.Id, orderId);

            return await _context
                            .Orders
                            .Find(matchId)
                            .FirstOrDefaultAsync();
        }

        public async Task<Order> CreateOrder(Order order)
        {
            order.CreationDateTime = DateTime.Now;
            order.OrderStatus = OrderStatus.New;

            await _context.Orders.InsertOneAsync(order);
            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerId(Guid customerId)
        {
            var matchId = Builders<Order>.Filter.Eq(o => o.CustomerInfo.CustomerId, customerId);

            return await _context
                            .Orders
                            .Find(matchId).ToListAsync();
        }
    }
}
