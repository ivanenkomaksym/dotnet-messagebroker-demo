using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Common.Models.Warehouse
{
    [Serializable]
    public record StockItem
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? Supplier { get; set; }

        [Required]
        public ushort Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public ushort Sold { get; set; }

        public ushort AvailableOnStock { get; set; }
    }
}
