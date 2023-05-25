using Common.Models.Payment;
using MongoDB.Driver;

namespace PaymentService.Data
{
    public interface IPaymentContext
    {
        IMongoCollection<Payment> Payments { get; }
    }
}
