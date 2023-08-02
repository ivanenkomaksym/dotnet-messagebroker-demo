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

        [Required]
        [Range(1, 5, ErrorMessage = "Only numbers from 1 to 5 are supported.")]
        public int Accuracy { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Only numbers from 1 to 5 are supported.")]
        public int Communication { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Only numbers from 1 to 5 are supported.")]
        public int DeliverySpeed { get; set; }
    }
}
