using Common.Models;

namespace Common.Events
{
    public record ShipOrder
    {
        public Guid OrderId { get; init; }

        public required CustomerInfo CustomerInfo { get; init; }

        public required Address ShippingAddress { get; init; }
    }
}