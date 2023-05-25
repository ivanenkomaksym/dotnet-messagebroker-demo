using Common.Models.Payment;

namespace PaymentService.Repositories
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetPayments();
        Task<Payment> GetPaymentByOrderId(Guid orderId);
        Task<Payment> GetPaymentByCustomerId(Guid customerId);

        Task<Payment> CreatePayment(Payment payment);
        Task<bool> UpdatePayment(Payment payment);
    }
}
