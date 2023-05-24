using Common.Models;

namespace Common.Events
{
    public record OrderCreated
    {
        public Guid OrderId { get; init; }

        public CustomerInfo CustomerInfo { get; init; }

        public OrderStatus OrderStatus { get; init; }

        public Address ShippingAddress { get; init; }

        public Payment Payment { get; init; }

        public IList<OrderItem> Items { get; init; } = new List<OrderItem>();

        public DateTime CreationDateTime { get; init; }
    }
}
