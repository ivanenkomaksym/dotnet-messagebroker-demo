namespace Common.Events
{
    public record OrderCollected
    {
        public Guid OrderId { get; init; }

        public DateTime CollectedDateTime { get; init; }
    }
}
