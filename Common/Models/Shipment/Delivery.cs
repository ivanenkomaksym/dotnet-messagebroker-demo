using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Common.Models.Shipment
{
    [Serializable]
    public record Delivery
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; }

        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid OrderId { get; set; }

        [Required]
        public CustomerInfo CustomerInfo { get; set; }

        [Required]
        public Address ShippingAddress { get; set; }

        [Required]
        public DeliveryStatus DeliveryStatus { get; set; } = DeliveryStatus.None;
    }
}
