using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ShoppingCartAPI.Entities
{
    public record ShoppingCartItem
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; init; }

        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid ProductId { get; init; }

        public string? ProductName { get; init; }
        public decimal ProductPrice { get; init; }
        public ushort Quantity { get; init; }
        public string? ImageFile { get; set; }
        public ushort AvailableOnStock { get; set; }
    }
}