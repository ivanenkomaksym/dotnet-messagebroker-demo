namespace Common.Models
{
    public enum OrderStatus
    {
        New,
        StockReserveFailed,
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
