using Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace WarehouseCommon.Data
{
    public class WarehouseContext : WarehouseContextBase
    {
        private readonly IWarehouseContextSeed _warehouseContextSeed;

        public WarehouseContext(IWarehouseContextSeed warehouseContextSeed, IConfiguration configuration, IOptions<DatabaseSettings> databaseSettings)
            : base(configuration, databaseSettings)
        {
            _warehouseContextSeed = warehouseContextSeed;
        }

        public sealed override async Task InitAsync()
        {
            await _warehouseContextSeed.SeedData(StockItems);
        }
    }
}