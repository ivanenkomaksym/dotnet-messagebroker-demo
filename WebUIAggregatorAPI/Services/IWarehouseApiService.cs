using Common.Models.Warehouse;

namespace WebUIAggregatorAPI.Services
{
    public interface IWarehouseApiService
    {
        public Task<StockItem> GetStockItemByProductId(Guid productId);
    }
}
