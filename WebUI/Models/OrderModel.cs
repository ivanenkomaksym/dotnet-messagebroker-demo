namespace WebUI.Models
{
    public record OrderModel
    {
        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public IList<OrderItemModel> Items { get; set; } = new List<OrderItemModel>();
        public double TotalPrice { get; set; }
        public BillingAddress BillingAddress { get; set; } 
        public Payment Payment { get; set; }
    }
}
