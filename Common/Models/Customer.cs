using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Common.Models.Payment;

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
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public UserRole UserRole { get; set; } = UserRole.User;

        public Address? ShippingAddress { get; set; }

        public PaymentInfo? PaymentInfo { get; set; }

        public DateTime CreationDateTime { get; set; } = DateTime.Now;
    }
}
