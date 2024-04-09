using System.ComponentModel.DataAnnotations;

namespace Common.Models.Payment
{
    public record PaymentInfo
    {
        [Required]
        public required string CardName { get; set; }

        [Required]
        public required string CardNumber { get; set; }

        [Required]
        public required string Expiration { get; set; }

        [Required]
        public required string CVV { get; set; }

        [Required]
        public required PaymentMethod PaymentMethod { get; set; }
    }
}
