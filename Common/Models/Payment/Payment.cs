using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Common.Models.Payment
{
    [Serializable]
    public record Payment
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; }

        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid OrderId { get; set; }

        [Required]
        public CustomerInfo CustomerInfo { get; set; }

        [Required]
        public PaymentInfo PaymentInfo { get; set; }

        public decimal PaidAmount;

        [Required]
        public DateTime CreatedOn { get;set; }

        public DateTime PaidDateTime;

        public PaymentStatus PaymentStatus { get; set; }
    }
}
