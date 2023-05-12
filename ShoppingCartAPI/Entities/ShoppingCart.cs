using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ShoppingCartAPI.Entities
{
    public record ShoppingCart
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; init; }

        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid CustomerId { get; init; }
        public string? CustomerName { get; init; }
        public IEnumerable<ShoppingCartItem> Items { get; init; }
        public double TotalPrice { get; init; }
    }
}
