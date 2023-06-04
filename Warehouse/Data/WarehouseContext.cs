using WarehouseCommon.Data;

namespace Warehouse.Data
{
    internal class WarehouseContext : WarehouseContextBase
    {
        public WarehouseContext(IWarehouseContextSeed warehouseContextSeed, IConfiguration configuration)
            : base(configuration)
        {
            warehouseContextSeed.SeedData(StockItems);
        }
    }
}
