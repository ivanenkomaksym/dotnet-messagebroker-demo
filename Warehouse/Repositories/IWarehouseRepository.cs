using Common.Models.Warehouse;

namespace Warehouse.Repositories
{
    public interface IWarehouseRepository
    {
        Task<IEnumerable<StockItem>> GetStockItems();
        Task<StockItem> GetStockItemByProductId(Guid productId);

        Task<StockItem> CreateStockItem(StockItem stockItem);
        Task<bool> DeleteStockItem(Guid id);

        Task<OrderReserve> CreateOrderReserve(OrderReserve orderReserve);
        Task<bool> DeleteReserve(Guid orderId);
    }
}
