using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    [Serializable]
    public record Customer
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
