using OrderCommon.Data;

namespace OrderCommon.Repositories
{
    internal class OrderRepository : OrderRepositoryBase
    {
        private bool _contextInit = false;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(IOrderContext context, ILogger<OrderRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public override async Task<IOrderContext> GetContext()
        {
            if (_contextInit)
                return _context;

            await _context.InitAsync();
            _contextInit = true;

            _logger.LogInformation($"Context initialized");
            return _context;
        }
    }
}