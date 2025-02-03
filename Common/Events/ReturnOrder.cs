namespace Common.Events
{
    public record ReturnOrder
    {
        public Guid OrderId { get; init; }

        public DateTime ReturnDateTime { get; init; }
    }
}