namespace Common.Events
{
    public record StockUpdated
    {
        public required IList<Guid> StockItemIds { get; set; }
    }
}
