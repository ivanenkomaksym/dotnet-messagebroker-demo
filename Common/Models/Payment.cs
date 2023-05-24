using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public record Payment
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
        public int PaymentMethod { get; set; }
    }
}
