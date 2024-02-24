using Microsoft.Extensions.Configuration;

namespace WarehouseCommon.Data
{
    public class WarehouseContext : WarehouseContextBase
    {
        private readonly IWarehouseContextSeed _warehouseContextSeed;

        public WarehouseContext(IWarehouseContextSeed warehouseContextSeed, IConfiguration configuration)
            : base(configuration)
        {
            _warehouseContextSeed = warehouseContextSeed;
        }

        public sealed override async Task InitAsync()
        {
            await _warehouseContextSeed.SeedData(StockItems);
        }
    }
}
