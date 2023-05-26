using Common.Models.Payment;
using Common.Models;

namespace Common.Events
{
    public record PaymentResult
    {
        public Guid OrderId { get; init; }

        public Guid PaymentId { get; init; }

        public CustomerInfo CustomerInfo { get; init; }

        public PaymentInfo PaymentInfo { get; init; }

        public PaymentStatus PaymentStatus { get; init; }
    }
}
