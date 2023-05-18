using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    [Serializable]
    public record OrderItem
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        [Required]
        public Guid ProductId { get; init; }
        public string? ProductName { get; init; }
        [Required]
        public double ProductPrice { get; init; }
        [Required]
        public ushort Quantity { get; set; }
    }
}
