using Common.Configuration;
using Microsoft.Extensions.Options;
using OrderCommon.Data;

namespace OrderAPI.Data
{
    internal class OrderContext : OrderContextBase
    {
        private readonly IOrderContextSeed _orderContextSeed;

        public OrderContext(IOrderContextSeed orderContextSeed, IOptions<DatabaseSettings> databaseSettings)
            : base(databaseSettings)
        {
            _orderContextSeed = orderContextSeed;
        }

        public sealed override async Task InitAsync()
        {
            await _orderContextSeed.SeedData(Orders);
        }
    }
}