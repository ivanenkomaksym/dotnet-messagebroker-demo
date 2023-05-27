using Common.Models.Payment;
using Common.Models;

namespace Common.Events
{
    public record OrderUpdated
    {
        public Guid OrderId { get; init; }

        public PaymentStatus PaymentStatus { get; set; }

        public CustomerInfo CustomerInfo { get; init; }

        public OrderStatus OrderStatus { get; init; }

        public Address ShippingAddress { get; init; }

        public PaymentInfo PaymentInfo { get; init; }

        public IList<OrderItem> Items { get; init; } = new List<OrderItem>();
    }
}
