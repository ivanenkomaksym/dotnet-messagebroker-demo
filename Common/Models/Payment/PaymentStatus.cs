namespace Common.Models.Payment
{
    public enum PaymentStatus
    {
        Unpaid,
        Failed,
        Expired,
        Paid,
        Refunding,
        Refunded
    }
}