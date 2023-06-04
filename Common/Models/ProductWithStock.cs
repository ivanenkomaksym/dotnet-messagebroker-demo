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

        // StockItem properties
        public ushort Quantity { get; set; }
        public decimal Discount { get; set; }
        public ushort Sold { get; set; }
        public ushort AvailableOnStock { get; set; }
    }
}
