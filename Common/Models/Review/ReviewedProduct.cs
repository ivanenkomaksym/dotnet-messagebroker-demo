using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Common.Models.Review
{
    [Serializable]
    public class ReviewedProduct
    {
        [Key]
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid ProductId { get; set; }

        [Required]
        public IList<Review> Reviews { get; set; } = new List<Review>();
    }
}
