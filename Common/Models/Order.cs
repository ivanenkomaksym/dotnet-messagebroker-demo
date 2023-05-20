using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    [Serializable]
    public record Order
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid CustomerId { get; set; }
        [Required]
        public IList<OrderItem> Items { get; set; } = new List<OrderItem>();
        [Required]
        public double TotalPrice { get; set; }
        [Required]
        public BillingAddress BillingAddress { get; set; }
        [Required]
        public Payment Payment { get; set; }
    }
}