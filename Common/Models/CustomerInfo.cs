using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Common.Models
{
    public record CustomerInfo
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid CustomerId { get; init; }

        public string? FirstName { get; init; }

        public string? LastName { get; init; }

        public string? Email { get; init; }
    }
}
