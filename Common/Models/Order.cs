using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    [Serializable]
    public record Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }
        [Required]
        public IList<OrderItem> Items { get; set; } = new List<OrderItem>();
        [Required]
        public double TotalPrice { get; set; }
        [Required]
        public BillingAddress BillingAddress { get; set; }
        [Required]
        public Payment Payment { get; set; }
    }
}