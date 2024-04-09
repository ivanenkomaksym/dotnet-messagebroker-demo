using Common.Models;

namespace Common.Events
{
    public class ShipmentToBeReturned
    {
        public Guid OrderId { get; init; }

        public required CustomerInfo CustomerInfo { get; init; }

        public required Address ShippingAddress { get; init; }
    }
}
