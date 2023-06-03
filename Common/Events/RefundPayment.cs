using Common.Models.Payment;
using Common.Models;

namespace Common.Events
{
    public record RefundPayment
    {
        public Guid OrderId { get; init; }

        public CustomerInfo CustomerInfo { get; init; }

        public PaymentInfo PaymentInfo { get; init; }

        public double TotalPrice { get; set; }
    }
}
