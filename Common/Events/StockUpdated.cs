namespace Common.Events
{
    public record StockUpdated
    {
        public IList<Guid> StockItemIds { get; set; }
    }
}
