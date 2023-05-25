using Common.Models.Warehouse;

namespace Common.Events
{
    public record StockReserved
    {
        public Guid OrderReserveId { get; init; }

        public Guid OrderId { get; init; }

        public IList<ReservedStockItem> ReservedStockItems { get; init; }
    }
}
