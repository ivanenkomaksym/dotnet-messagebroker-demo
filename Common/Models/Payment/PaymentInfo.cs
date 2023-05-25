using System.ComponentModel.DataAnnotations;

namespace Common.Models.Payment
{
    public record PaymentInfo
    {
        [Required]
        public string CardName { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        public string Expiration { get; set; }

        [Required]
        public string CVV { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }
}
