namespace Common.Models.Warehouse
{
    public record FailedToReserveProduct
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public ushort AvailableOnStock { get; set; }
    }
}
