namespace Common.Models
{
    public class ProductWithStock
    {
        // Product properties
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Category { get; set; }
        public required string Summary { get; set; }
        public required string ImageFile { get; set; }
        public decimal Price { get; set; }

        public decimal DiscountedPrice
        {
            get
            {
                return Price - Price * Discount;
            }
            init { }
        }

        public decimal Rating
        {
            get
            {
                var rand = new Random();
                return decimal.Round((decimal)(5.0 - rand.NextDouble()), 1);
            }
            init { }
        }

        // StockItem properties
        public Guid StockItemId { get; set; }
        public ushort Quantity { get; set; }
        public decimal Discount { get; set; }
        public ushort Sold { get; set; }
        public ushort AvailableOnStock { get; set; }
    }
}
