using System.Text;
using Common;
using Common.Configuration;
using Common.Persistence;
using RabbitMQ.Client;

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

            var warehouseOrderStatusQueue = Consts.GetOrderStatusQueueName("warehouse");

            var channel = rabbitMQChannelRegistry.GetOrCreateQueue(rabbitMQOptions.HostName, rabbitMQOptions.Port, warehouseOrderStatusQueue, (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    _logger.LogInformation($"['{warehouseOrderStatusQueue}' queue] Received '{message}' with routingKey '{routingKey}'");
                });

            // Bind new worker queue to "exchange.order.status" exchange
            // so that only one instance of this service will receive a message and process it
            channel.QueueBind(queue: warehouseOrderStatusQueue,
                              exchange: Consts.OrderStatusExchange,
                              routingKey: Consts.OrderStatusBindingKey);
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
