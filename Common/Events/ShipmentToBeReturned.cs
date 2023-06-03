using Common.Models;

namespace Common.Events
{
    public class ShipmentToBeReturned
    {
        public Guid OrderId { get; init; }

        public CustomerInfo CustomerInfo { get; init; }

        public Address ShippingAddress { get; init; }
    }
}
