namespace Common.Models
{
    public enum OrderStatus
    {
        New,
        PaymentProcessing,
        PaymentFailed,
        Paid,
        AwaitingShipment,
        Shipping,
        AwaitingCollection,
        Completed,
        AwaitingReturn,
        Refunding,
        Refunded,
        Cancelled
    }
}
