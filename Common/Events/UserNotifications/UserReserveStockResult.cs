using Common.Models.Warehouse;

namespace Common.Events.UserNotifications
{
    public record UserReserveStockResult
    {
        public Guid CustomerId { get; init; }

        public Guid OrderId { get; init; }

        public ReserveResult ReserveResult { get; init; }

        public IList<FailedToReserveProduct> FailedToReserveProducts { get; init; }
    }
}
