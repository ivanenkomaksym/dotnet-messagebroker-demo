using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Common.Models.Review
{
    [Serializable]
    public class ReviewDetails
    {
        [Key]
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Range(1, 5, ErrorMessage = "Only numbers from 1 to 5 are supported.")]
        public int? Accuracy { get; set; }

        [Range(1, 5, ErrorMessage = "Only numbers from 1 to 5 are supported.")]
        public int? Communication { get; set; }

        [Range(1, 5, ErrorMessage = "Only numbers from 1 to 5 are supported.")]
        public int? DeliverySpeed { get; set; }
    }
}
