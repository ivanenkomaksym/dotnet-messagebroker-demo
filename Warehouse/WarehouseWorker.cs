using System.Text;
using Common;
using Common.Configuration;
using Common.Persistence;

namespace Warehouse
{
    public sealed class WarehouseWorker : BackgroundService
    {
        private readonly ILogger<WarehouseWorker> _logger;

        public WarehouseWorker(IConfiguration configuration, ILogger<WarehouseWorker> logger)
        {
            _logger = logger;

            var rabbitMQOptions = configuration.GetSection(RabbitMQOptions.Name).Get<RabbitMQOptions>();

            IRabbitMQChannelRegistry rabbitMQChannelRegistry;
            if (rabbitMQOptions.UseStub)
                rabbitMQChannelRegistry = new StubRabbitMQChannelRegistry();
            else
                rabbitMQChannelRegistry = new RabbitMQChannelRegistry();

            rabbitMQChannelRegistry.GetOrCreateQueue(rabbitMQOptions.HostName, rabbitMQOptions.Port, Consts.OrderPaidQueue, (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($" [x] Received {message}");
            });
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
