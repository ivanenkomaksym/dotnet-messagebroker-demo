namespace Common.Models
{
    public class ProductWithStock
    {
        // Product properties
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Summary { get; set; }
        public string? ImageFile { get; set; }
        public double Price { get; set; }

        public double DiscountedPrice
        {
            get
            {
                return Price - Price * Discount;
            }
            init { }
        }

        public double Rating
        {
            get
            {
                var rand = new Random();
                return 5.0 - rand.NextDouble();
            }
            init { }
        }

        // StockItem properties
        public Guid StockItemId { get; set; }
        public ushort Quantity { get; set; }
        public double Discount { get; set; }
        public ushort Sold { get; set; }
        public ushort AvailableOnStock { get; set; }
    }
}
