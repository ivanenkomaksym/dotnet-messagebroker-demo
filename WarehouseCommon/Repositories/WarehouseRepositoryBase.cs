using Common.Models.Warehouse;
using MongoDB.Driver;
using WarehouseCommon.Data;

namespace WarehouseCommon.Repositories
{
    public class WarehouseRepositoryBase : IWarehouseRepository
    {
        protected readonly IWarehouseContext _context;

        public WarehouseRepositoryBase(IWarehouseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public virtual Task<IWarehouseContext> GetContext()
        {
            return Task.FromResult(_context);
        }

        public async Task<StockItem> CreateStockItem(StockItem stockItem)
        {
            var context = await GetContext();
            await context.StockItems.InsertOneAsync(stockItem);
            return stockItem;
        }

        public async Task<bool> UpdateStockItem(StockItem stockItem)
        {
            var context = await GetContext();
            var updateResult = await context
                                        .StockItems
                                        .ReplaceOneAsync(filter: s => s.Id == stockItem.Id, replacement: stockItem);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> DeleteReserve(Guid orderId)
        {
            var context = await GetContext();
            var filter = Builders<OrderReserve>.Filter.Eq(o => o.OrderId, orderId);

            DeleteResult deleteResult = await context
                                                .OrderReserves
                                                .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<bool> DeleteStockItem(Guid id)
        {
            var context = await GetContext();
            var filter = Builders<StockItem>.Filter.Eq(s => s.Id, id);

            DeleteResult deleteResult = await context
                                                .StockItems
                                                .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<StockItem> GetStockItemByProductId(Guid productId)
        {
            var context = await GetContext();

            var stockItems = context.StockItems;

            var matchProductId = Builders<StockItem>.Filter.Eq(s => s.ProductId, productId);

            return await stockItems.Find(matchProductId)
                            .FirstOrDefaultAsync();
        }

        public async Task<StockItem> GetStockItemById(Guid stockItemId)
        {
            var context = await GetContext();
            var matchStockItemId = Builders<StockItem>.Filter.Eq(s => s.Id, stockItemId);

            return await context
                            .StockItems
                            .Find(matchStockItemId)
                            .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<StockItem>> GetStockItems()
        {
            var context = await GetContext();
            return await context
                            .StockItems
                            .Find(s => true)
                            .ToListAsync();
        }

        public async Task<OrderReserve> CreateOrderReserve(OrderReserve orderReserve)
        {
            var context = await GetContext();
            await context.OrderReserves.InsertOneAsync(orderReserve);
            return orderReserve;
        }

        public async Task<OrderReserve> GetOrderReserveByOrderId(Guid orderId)
        {
            var context = await GetContext();
            var matchOrderId = Builders<OrderReserve>.Filter.Eq(o => o.OrderId, orderId);

            return await context
                            .OrderReserves
                            .Find(matchOrderId)
                            .FirstOrDefaultAsync();
        }
    }
}