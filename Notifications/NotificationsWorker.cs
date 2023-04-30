using System.Text;
using Common;
using Common.Configuration;
using Common.Persistence;
using RabbitMQ.Client;

namespace Notifications
{
    public sealed class NotificationsWorker : BackgroundService
    {
        public NotificationsWorker(IConfiguration configuration, ILogger<NotificationsWorker> logger)
        {
            Logger = logger;

            RabbitMQOptions = configuration.GetSection(RabbitMQOptions.Name).Get<RabbitMQOptions>();

            if (RabbitMQOptions.UseStub)
                RabbitMQChannelRegistry = new StubRabbitMQChannelRegistry();
            else
                RabbitMQChannelRegistry = new RabbitMQChannelRegistry();

            bindToOrderStatusQueue();

            bindToCustomerStatusQueue();
        }

        private void bindToOrderStatusQueue()
        {
            var notificationsOrderStatusQueue = Consts.GetOrderStatusQueueName("notifications");

            var orderStatusChannel = RabbitMQChannelRegistry.GetOrCreateQueue(RabbitMQOptions.HostName, RabbitMQOptions.Port, notificationsOrderStatusQueue, (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                Logger.LogInformation($"['{notificationsOrderStatusQueue}' queue] Received '{message}' with routingKey '{routingKey}'");
            });

            // Bind new worker queue to "exchange.order.status" exchange
            // so that only one instance of this service will receive a message and process it
            orderStatusChannel.QueueBind(queue: notificationsOrderStatusQueue,
                                         exchange: Consts.OrderStatusExchange,
                                         routingKey: Consts.OrderStatusBindingKey);
        }

        private void bindToCustomerStatusQueue()
        {
            var notificationsCustomerStatusQueue = Consts.GetCustomerStatusQueueName("notifications");

            var customerStatusChannel = RabbitMQChannelRegistry.GetOrCreateQueue(RabbitMQOptions.HostName, RabbitMQOptions.Port, notificationsCustomerStatusQueue, (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                Logger.LogInformation($"['{notificationsCustomerStatusQueue}' queue] Received '{message}' with routingKey '{routingKey}'");
            });

            // Bind new worker queue to "exchange.customer.status" exchange
            // so that only one instance of this service will receive a message and process it
            customerStatusChannel.QueueBind(queue: notificationsCustomerStatusQueue,
                                            exchange: Consts.CustomerStatusExchange,
                                            routingKey: Consts.CustomerStatusBindingKey);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private readonly IRabbitMQChannelRegistry RabbitMQChannelRegistry;
        private readonly RabbitMQOptions RabbitMQOptions;
        private readonly ILogger<NotificationsWorker> Logger;
    }
}
