using OrderCommon.Data;

namespace OrderAPI.Data
{
    internal class OrderContext : OrderContextBase
    {
        private readonly IOrderContextSeed _orderContextSeed;

        public OrderContext(IOrderContextSeed orderContextSeed, IConfiguration configuration)
            : base(configuration)
        {
            _orderContextSeed = orderContextSeed;
        }

        public sealed override async Task InitAsync()
        {
            await _orderContextSeed.SeedData(Orders);
        }
    }
}
