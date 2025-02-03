namespace Common.Events
{
    public record CancelOrder
    {
        public Guid OrderId { get; init; }

        public DateTime CancelDateTime { get; init; }
    }
}