using Common.Models;
using Common.Models.Warehouse;

namespace Common.Events
{
    public record ReserveStockResult
    {
        public Guid OrderReserveId { get; init; }

        public Guid OrderId { get; init; }

        public CustomerInfo? CustomerInfo { get; init; }

        public ReserveResult? ReserveResult { get; init; }

        public IList<ReservedStockItem>? ReservedStockItems { get; init; }

        public IList<FailedToReserveProduct>? FailedToReserveProducts { get; init; }
    }
}
