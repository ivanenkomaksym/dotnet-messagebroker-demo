namespace Common.Events
{
    public record ReserveRemoved
    {
        public Guid OrderId { get; init; }
    }
}