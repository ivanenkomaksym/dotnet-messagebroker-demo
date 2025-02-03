using System.ComponentModel.DataAnnotations;

namespace Common.Models.Warehouse
{
    public record FailedToReserveProduct
    {
        public Guid ProductId { get; set; }

        [Required]
        public required string ProductName { get; set; }

        public ushort AvailableOnStock { get; set; }
    }
}