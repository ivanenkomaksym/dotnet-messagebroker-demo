using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
        public required CustomerInfo CustomerInfo { get; set; }

        [Required]
        public required Address ShippingAddress { get; set; }

        [Required]
        public DeliveryStatus DeliveryStatus { get; set; } = DeliveryStatus.None;
    }
}