using Common.Models;

namespace Common.Events
{
    public record ReserveStock
    {
        public Guid OrderId { get; init; }

        public CustomerInfo? CustomerInfo { get; init; }

        public required IList<OrderItem> Items { get; init; }
    }
}