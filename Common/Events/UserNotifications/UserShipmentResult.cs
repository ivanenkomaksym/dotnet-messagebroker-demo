using Common.Models.Shipment;

namespace Common.Events.UserNotifications
{
    public record UserShipmentResult
    {
        public Guid CustomerId { get; init; }

        public Guid OrderId { get; init; }

        public DeliveryStatus DeliveryStatus { get; init; }
    }
}
