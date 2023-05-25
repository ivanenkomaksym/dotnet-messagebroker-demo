using Common.Models.Payment;
using PaymentService.Data;
using MongoDB.Driver;

namespace PaymentService.Repositories
{
    internal class PaymentRepository : IPaymentRepository
    {
        private readonly IPaymentContext _context;

        public PaymentRepository(IPaymentContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Payment>> GetPayments()
        {
            return await _context
                            .Payments
                            .Find(p => true)
                            .ToListAsync();
        }

        public async Task<Payment> GetPaymentByOrderId(Guid orderId)
        {
            var matchOrderId = Builders<Payment>.Filter.Eq(p => p.OrderId, orderId);

            return await _context
                            .Payments
                            .Find(matchOrderId)
                            .FirstOrDefaultAsync();
        }

        public async Task<Payment> GetPaymentByCustomerId(Guid customerId)
        {
            var matchCustomerId = Builders<Payment>.Filter.Eq(p => p.CustomerInfo.CustomerId, customerId);

            return await _context
                            .Payments
                            .Find(matchCustomerId)
                            .FirstOrDefaultAsync();
        }

        public async Task<Payment> CreatePayment(Payment payment)
        {
            await _context.Payments.InsertOneAsync(payment);
            return payment;
        }

        public async Task<bool> UpdatePayment(Payment payment)
        {
            var updateResult = await _context
                                        .Payments
                                        .ReplaceOneAsync(filter: p => p.Id == payment.Id, replacement: payment);

            return updateResult.IsAcknowledged
                    && updateResult.ModifiedCount > 0;
        }
    }
}
