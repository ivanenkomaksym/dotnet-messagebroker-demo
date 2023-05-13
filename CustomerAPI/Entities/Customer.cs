using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace CustomerAPI.Entities
{
    [Serializable]
    public record Customer
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [BsonElement("Name")]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
