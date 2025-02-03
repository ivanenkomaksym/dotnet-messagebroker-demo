using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Models.Warehouse
{
    [Serializable]
    public record ReservedStockItem
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; }

        [Required]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid StockItemId { get; set; }

        [Required]
        public ushort Reserved { get; set; }
    }
}