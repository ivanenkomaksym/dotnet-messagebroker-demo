using System.ComponentModel.DataAnnotations;
using Common.Models.Payment;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Models
{
    [Serializable]
    public record Customer
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [BsonElement("Name")]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        public UserRole UserRole { get; set; } = UserRole.User;

        public required Address ShippingAddress { get; set; }

        public required PaymentInfo PaymentInfo { get; set; }

        public DateTime CreationDateTime { get; set; } = DateTime.Now;
    }
}