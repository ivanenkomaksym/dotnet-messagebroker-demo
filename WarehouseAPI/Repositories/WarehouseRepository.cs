using WarehouseCommon.Data;
using WarehouseCommon.Repositories;

namespace WarehouseAPI.Data
{
    public sealed class WarehouseRepository : WarehouseRepositoryBase
    {
        private bool _contextInit = false;
        private readonly ILogger<WarehouseRepository> _logger;

        public WarehouseRepository(IWarehouseContext context, ILogger<WarehouseRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public override async Task<IWarehouseContext> GetContext()
        {
            if (_contextInit)
                return _context;

            _contextInit = true;
            await _context.InitAsync();

            _logger.LogInformation($"Context initialized");
            return _context;
        }
    }
}
