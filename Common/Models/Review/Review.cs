using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Common.Models.Review
{
    [Serializable]
    public class Review
    {
        [Key]
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public required CustomerInfo CustomerInfo { get; set; }

        [Required]
        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Only numbers from 1 to 5 are supported.")]
        public int Rating { get; set; }

        [BsonDefaultValue(false)]
        public bool? Anonymous { get; set; } = false;

        [BsonDefaultValue(null)]
        public ReviewDetails? ReviewDetails { get; set; }

        [BsonDefaultValue(null)]
        public DateTime? CreationDateTime { get; set; } = DateTime.Now;
    }
}
