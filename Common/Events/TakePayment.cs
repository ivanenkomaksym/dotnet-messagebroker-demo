using Common.Models;
using Common.Models.Payment;

namespace Common.Events
{
    public record TakePayment
    {
        public Guid OrderId { get; init; }

        public CustomerInfo CustomerInfo { get; init; }

        public PaymentInfo PaymentInfo { get; init; }
    }
}
