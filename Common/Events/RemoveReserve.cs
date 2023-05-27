namespace Common.Events
{
    public enum RemoveReserveReason
    {
        PaymentFailed,
        PaymentExpired,
        OrderCancelled,
        TakeFromStockForShipment
    }

    public record RemoveReserve
    {
        public Guid OrderId { get; init; }

        public RemoveReserveReason Reason { get; init; }
    }
}
