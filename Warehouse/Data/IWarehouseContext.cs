using Common.Models.Warehouse;
using MongoDB.Driver;

namespace Warehouse.Data
{
    public interface IWarehouseContext
    {
        IMongoCollection<StockItem> StockItems { get; }

        IMongoCollection<OrderReserve> OrderReserves { get; }
    }
}
