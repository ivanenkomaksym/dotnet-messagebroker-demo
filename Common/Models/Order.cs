using System.ComponentModel.DataAnnotations;
using Common.Models.Payment;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Models
{
    [Serializable]
    public record Order
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public OrderStatus OrderStatus { get; set; } = OrderStatus.New;

        [Required]
        public required CustomerInfo CustomerInfo { get; set; }

        [Required]
        public IList<OrderItem> Items { get; set; } = new List<OrderItem>();

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public required Address ShippingAddress { get; set; }

        [Required]
        public required PaymentInfo PaymentInfo { get; set; }

        public decimal UseCashback { get; set; }

        public DateTime CreationDateTime { get; set; } = DateTime.UtcNow;
    }
}