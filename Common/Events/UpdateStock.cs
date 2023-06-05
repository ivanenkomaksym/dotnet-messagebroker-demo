using Common.Models;

namespace Common.Events
{
    public record UpdateStock
    {
        public Guid OrderId { get; init; }

        public IList<OrderItem> Items { get; init; }
    }
}
