using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Models
{
    public class Product
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; }

        [BsonElement("Name")]
        public string? Name { get; set; }
        public string? Author { get; set; }
        public string? Category { get; set; }
        public string? Summary { get; set; }
        public string? ImageFile { get; set; }
        public decimal Price { get; set; }
    }
}
