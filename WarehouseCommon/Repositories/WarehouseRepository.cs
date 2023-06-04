﻿using Common.Models.Warehouse;
using WarehouseCommon.Data;
using MongoDB.Driver;

namespace WarehouseCommon.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly IWarehouseContext _context;

        public WarehouseRepository(IWarehouseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<StockItem> CreateStockItem(StockItem stockItem)
        {
            await _context.StockItems.InsertOneAsync(stockItem);
            return stockItem;
        }

        public async Task<bool> UpdateStockItem(StockItem stockItem)
        {
            var updateResult = await _context
                                        .StockItems
                                        .ReplaceOneAsync(filter: s => s.Id == stockItem.Id, replacement: stockItem);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> DeleteReserve(Guid orderId)
        {
            var filter = Builders<OrderReserve>.Filter.Eq(o => o.OrderId, orderId);

            DeleteResult deleteResult = await _context
                                                .OrderReserves
                                                .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<bool> DeleteStockItem(Guid id)
        {
            var filter = Builders<StockItem>.Filter.Eq(s => s.Id, id);

            DeleteResult deleteResult = await _context
                                                .StockItems
                                                .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<StockItem> GetStockItemByProductId(Guid productId)
        {
            var matchProductId = Builders<StockItem>.Filter.Eq(s => s.ProductId, productId);

            return await _context
                            .StockItems
                            .Find(matchProductId)
                            .FirstOrDefaultAsync();
        }

        public async Task<StockItem> GetStockItemById(Guid stockItemId)
        {
            var matchStockItemId = Builders<StockItem>.Filter.Eq(s => s.Id, stockItemId);

            return await _context
                            .StockItems
                            .Find(matchStockItemId)
                            .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<StockItem>> GetStockItems()
        {
            return await _context
                            .StockItems
                            .Find(s => true)
                            .ToListAsync();
        }

        public async Task<OrderReserve> CreateOrderReserve(OrderReserve orderReserve)
        {
            await _context.OrderReserves.InsertOneAsync(orderReserve);
            return orderReserve;
        }
        
        public async Task<OrderReserve> GetOrderReserveByOrderId(Guid orderId)
        {
            var matchOrderId = Builders<OrderReserve>.Filter.Eq(o => o.OrderId, orderId);

            return await _context
                            .OrderReserves
                            .Find(matchOrderId)
                            .FirstOrDefaultAsync();
        }
    }
}