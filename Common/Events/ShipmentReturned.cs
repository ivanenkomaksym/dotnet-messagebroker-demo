using Common.Models;
using Common.Models.Shipment;

namespace Common.Events
{
    public record ShipmentReturned
    {
        public Guid OrderId { get; init; }

        public required CustomerInfo CustomerInfo { get; init; }

        public DeliveryStatus DeliveryStatus { get; init; }
    }
}
