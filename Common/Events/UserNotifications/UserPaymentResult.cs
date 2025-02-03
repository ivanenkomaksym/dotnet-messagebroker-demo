using Common.Models.Payment;

namespace Common.Events.UserNotifications
{
    public record UserPaymentResult
    {
        public Guid CustomerId { get; init; }

        public Guid OrderId { get; init; }

        public PaymentStatus PaymentStatus { get; init; }
    }
}