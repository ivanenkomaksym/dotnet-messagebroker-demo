using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public record BillingAddress
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string AddressLine { get; set; }
        [Required]
        public string Country { get; set; }
        public string State { get; set; }
        [Required]
        public string ZipCode { get; set; }
    }
}
