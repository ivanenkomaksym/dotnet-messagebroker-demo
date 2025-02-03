namespace WebUI.Models
{
    public record ShoppingCartModel
    {
        public Guid Id { get; init; }
        public Guid CustomerId { get; init; }
        public string? CustomerName { get; init; }
        public List<ShoppingCartItemModel> Items { get; set; } = new List<ShoppingCartItemModel>();
        public decimal TotalPrice
        {
            get
            {
                decimal totalPrice = 0;
                foreach (var item in Items)
                {
                    totalPrice += item.ProductPrice * item.Quantity;
                }
                return totalPrice;
            }
            init
            {

            }
        }
    }
}