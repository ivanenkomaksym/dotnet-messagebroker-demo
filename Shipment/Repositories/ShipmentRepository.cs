using Common.Models.Shipment;
using MongoDB.Driver;
using Shipment.Data;

namespace Shipment.Repositories
{
    internal class ShipmentRepository : IShipmentRepository
    {
        private readonly IShipmentContext _context;

        public ShipmentRepository(IShipmentContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Delivery>> GetDeliveries()
        {
            return await _context
                            .Deliveries
                            .Find(p => true)
                            .ToListAsync();
        }

        public async Task<Delivery> GetDeliveryByOrderId(Guid orderId)
        {
            var matchOrderId = Builders<Delivery>.Filter.Eq(d => d.OrderId, orderId);

            return await _context
                            .Deliveries
                            .Find(matchOrderId)
                            .FirstOrDefaultAsync();
        }

        public async Task<Delivery> GetDeliveryByCustomerId(Guid customerId)
        {
            var matchCustomerId = Builders<Delivery>.Filter.Eq(d => d.CustomerInfo.CustomerId, customerId);

            return await _context
                            .Deliveries
                            .Find(matchCustomerId)
                            .FirstOrDefaultAsync();
        }

        public async Task<Delivery> CreateDelivery(Delivery Delivery)
        {
            await _context.Deliveries.InsertOneAsync(Delivery);
            return Delivery;
        }

        public async Task<bool> UpdateDelivery(Delivery Delivery)
        {
            var updateResult = await _context
                                        .Deliveries
                                        .ReplaceOneAsync(filter: p => p.Id == Delivery.Id, replacement: Delivery);

            return updateResult.IsAcknowledged
                    && updateResult.ModifiedCount > 0;
        }
    }
}