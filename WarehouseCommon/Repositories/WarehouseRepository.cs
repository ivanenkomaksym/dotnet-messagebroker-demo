using Microsoft.Extensions.Logging;
using WarehouseCommon.Repositories;

namespace WarehouseCommon.Data
{
    public sealed class WarehouseRepository : WarehouseRepositoryBase
    {
        private bool _contextInit = false;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
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

            await _semaphore.WaitAsync(); // Only one thread can proceed
            try
            {
                if (!_contextInit)
                {

                    await _context.InitAsync();
                    _logger.LogInformation($"Context initialized");
                    _contextInit = true;
                }
            }
            finally
            {
                _semaphore.Release();
            }

            return _context;
        }
    }
}
