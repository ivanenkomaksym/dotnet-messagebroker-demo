using System.Text;
using Common;
using Common.Configuration;
using Common.Persistence;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace OrderProcessor
{
    public sealed class OrderProcessorWorker : BackgroundService
    {
        private readonly ILogger<OrderProcessorWorker> _logger;

        public OrderProcessorWorker(IConfiguration configuration, ILogger<OrderProcessorWorker> logger)
        {
            _logger = logger;

            RabbitMQOptions = configuration.GetSection(RabbitMQOptions.Name).Get<RabbitMQOptions>();

            if (RabbitMQOptions.UseStub)
                RabbitMQChannelRegistry = new StubRabbitMQChannelRegistry();
            else
                RabbitMQChannelRegistry = new RabbitMQChannelRegistry();

            NewOrderChannel = RabbitMQChannelRegistry.GetOrCreateQueue(RabbitMQOptions.HostName, RabbitMQOptions.Port, Consts.NewOrderQueue, (model, ea) => { NewOrderRequested(ea); });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void NewOrderRequested(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation($" [x] Received {message}");

            // TODO: OrderProcessor redirects client to pay for the order and after that notifies consumers

            NotifyConsumersAboutOrderPaid();
        }

        private void NotifyConsumersAboutOrderPaid()
        {
            var orderPaidExchange = RabbitMQChannelRegistry.GetOrCreateExchange(RabbitMQOptions.HostName, RabbitMQOptions.Port, Consts.OrderStatusExchange, string.Empty, null);

            const string message = "Order paid";
            var body = Encoding.UTF8.GetBytes(message);

            orderPaidExchange.BasicPublish(exchange: Consts.OrderStatusExchange,
                                           routingKey: Consts.OrderStatusPaid,
                                           basicProperties: null,
                                           body: body);

            _logger.LogInformation($" [x] Sent {message}");
        }

        private readonly IRabbitMQChannelRegistry RabbitMQChannelRegistry;
        private readonly RabbitMQOptions RabbitMQOptions;
        private readonly IModel NewOrderChannel;
    }
}
