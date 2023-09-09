namespace WebUI.Models
{
    public record ShoppingCartModel
    {
        public Guid Id { get; init; }
        public Guid CustomerId { get; init; }
        public string? CustomerName { get; init; }
        public List<ShoppingCartItemModel> Items { get; set; } = new List<ShoppingCartItemModel>();
        public decimal TotalPrice { get; set; }
    }
}
