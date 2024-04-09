using Common.Models;
using Common.Models.Payment;

namespace Common.Events
{
    public record OrderCreated
    {
        public Guid OrderId { get; init; }

        public required CustomerInfo CustomerInfo { get; init; }

        public required OrderStatus OrderStatus { get; init; }

        public required Address ShippingAddress { get; init; }

        public required PaymentInfo PaymentInfo { get; init; }

        public required IList<OrderItem> Items { get; init; } = new List<OrderItem>();

        public DateTime CreationDateTime { get; init; }
    }
}
