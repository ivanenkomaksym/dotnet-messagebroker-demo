using Warehouse.Data;

namespace Warehouse
{
    public sealed class WarehouseWorker : BackgroundService
    {
        private readonly ILogger<WarehouseWorker> _logger;

        public WarehouseWorker(IWarehouseContext warehouseContext, ILogger<WarehouseWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
