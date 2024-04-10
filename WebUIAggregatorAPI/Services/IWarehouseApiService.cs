using Common.Models.Warehouse;

namespace WebUIAggregatorAPI.Services
{
    public interface IWarehouseApiService
    {
        public Task<StockItem?> GetStockItemByProductId(Guid productId);

        public Task<StockItem?> CreateStockItem(StockItem stockItem);

        public Task<StockItem?> UpdateStockItem(StockItem stockItem);

        public Task<bool> DeleteStockItemByProductId(Guid productId);
    }
}
