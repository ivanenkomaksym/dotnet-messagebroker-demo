using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

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
        public double ProductPrice { get; init; }
        public ushort Quantity { get; init; }
    }
}
