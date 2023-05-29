using Common.Models.Warehouse;

namespace Warehouse.Repositories
{
    public interface IWarehouseRepository
    {
        Task<IEnumerable<StockItem>> GetStockItems();
        Task<StockItem> GetStockItemByProductId(Guid productId);
        Task<StockItem> GetStockItemById(Guid stockItemId);

        Task<StockItem> CreateStockItem(StockItem stockItem);
        Task<bool> UpdateStockItem(StockItem stockItem);
        Task<bool> DeleteStockItem(Guid id);

        Task<OrderReserve> CreateOrderReserve(OrderReserve orderReserve);
        Task<OrderReserve> GetOrderReserveByOrderId(Guid orderId);
        Task<bool> DeleteReserve(Guid orderId);
    }
}
