using Common.Models;

namespace Common.Events
{
    public record ShipOrder
    {
        public Guid OrderId { get; init; }

        public CustomerInfo CustomerInfo { get; init; }

        public Address ShippingAddress { get; init; }
    }
}
