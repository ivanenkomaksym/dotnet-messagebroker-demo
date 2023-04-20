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

            rabbitMQChannelRegistry.GetOrCreateExchange(rabbitMQOptions.HostName, rabbitMQOptions.Port, Consts.OrderStatusExchange, Consts.OrderStatusBindingKey, (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                _logger.LogInformation($"['{Consts.OrderStatusExchange}' exchange] Received '{message}' with routingKey '{routingKey}'");
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
