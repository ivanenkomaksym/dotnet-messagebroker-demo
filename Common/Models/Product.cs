using System.Text.Json.Serialization;
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
        public required string Name { get; set; }
        public string? Author { get; set; }
        public required string Category { get; set; }
        public required string Summary { get; set; }
        public required string ImageFile { get; set; }
        public decimal Price { get; set; }

        /// <summary>Optional embedding for the catalog item's description.</summary>
        [JsonIgnore]
        public float[] Embedding { get; set; }

        [JsonIgnore]
        public double Score { get; set; }
    }
}