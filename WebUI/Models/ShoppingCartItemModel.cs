namespace WebUI.Models
{
    public record ShoppingCartItemModel
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public string? ProductName { get; init; }
        public double ProductPrice { get; init; }
        public ushort Quantity { get; set; }
        public string? ImageFile { get; set; }
        public ushort AvailableOnStock { get; set; }
    }
}
