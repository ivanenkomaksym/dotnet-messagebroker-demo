namespace WebUI.Models
{
    public record ShoppingCartItemModel
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public required string ProductName { get; init; }
        public decimal ProductPrice { get; init; }
        public ushort Quantity { get; set; }
        public string? ImageFile { get; set; }
        public ushort AvailableOnStock { get; set; }
    }
}
