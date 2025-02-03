using Common.Models;
using Common.Models.Payment;

namespace Common.Events
{
    public record PaymentRefunded
    {
        public Guid OrderId { get; init; }

        public Guid PaymentId { get; init; }

        public required CustomerInfo CustomerInfo { get; init; }

        public required PaymentInfo PaymentInfo { get; init; }

        public PaymentStatus PaymentStatus { get; init; }
    }
}