using System.Text;
using Common;
using Common.Configuration;
using Common.Persistence;
using RabbitMQ.Client;

namespace Notifications
{
    public sealed class NotificationsWorker : BackgroundService
    {
        private readonly ILogger<NotificationsWorker> _logger;

        public NotificationsWorker(IConfiguration configuration, ILogger<NotificationsWorker> logger)
        {
            _logger = logger;

            var rabbitMQOptions = configuration.GetSection(RabbitMQOptions.Name).Get<RabbitMQOptions>();

            IRabbitMQChannelRegistry rabbitMQChannelRegistry;
            if (rabbitMQOptions.UseStub)
                rabbitMQChannelRegistry = new StubRabbitMQChannelRegistry();
            else
                rabbitMQChannelRegistry = new RabbitMQChannelRegistry();

            var notificationsOrderStatusQueue = Consts.GetOrderStatusQueueName("notifications");

            var channel = rabbitMQChannelRegistry.GetOrCreateQueue(rabbitMQOptions.HostName, rabbitMQOptions.Port, notificationsOrderStatusQueue, (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    _logger.LogInformation($"['{notificationsOrderStatusQueue}' queue] Received '{message}' with routingKey '{routingKey}'");
                });

            // Bind new worker queue to "exchange.order.status" exchange
            // so that only one instance of this service will receive a message and process it
            channel.QueueBind(queue: notificationsOrderStatusQueue,
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
