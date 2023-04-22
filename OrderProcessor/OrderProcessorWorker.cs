using System.Text;
using System.Text.Json;
using Common;
using Common.Configuration;
using Common.Models;
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

            NewOrderChannel = RabbitMQChannelRegistry.GetOrCreateQueue(RabbitMQOptions.HostName, RabbitMQOptions.Port, Consts.OrderQueue, (model, ea) => { NewOrderRequested(ea); });

            OrderPaidExchange = RabbitMQChannelRegistry.GetOrCreateExchange(RabbitMQOptions.HostName, RabbitMQOptions.Port, Consts.OrderStatusExchange, ExchangeType.Fanout, string.Empty, null);
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
            var order = JsonSerializer.Deserialize<Order>(message);
            _logger.LogInformation($"['{Consts.OrderQueue}' queue] Received '{message}'");

            // TODO: OrderProcessor redirects client to pay for the order and after that notifies consumers

            NotifyConsumersAboutOrderPaid(order);
        }

        private void NotifyConsumersAboutOrderPaid(Order order)
        {
            var message = JsonSerializer.Serialize(order);
            var body = Encoding.UTF8.GetBytes(message);

            OrderPaidExchange.BasicPublish(exchange: Consts.OrderStatusExchange,
                                           routingKey: Consts.OrderStatusPaid,
                                           basicProperties: null,
                                           body: body);

            _logger.LogInformation($"['{Consts.OrderStatusExchange}' exchange] Sent '{message}' with routingKey '{Consts.OrderStatusPaid}'");
        }

        private readonly IRabbitMQChannelRegistry RabbitMQChannelRegistry;
        private readonly RabbitMQOptions RabbitMQOptions;
        private readonly IModel NewOrderChannel;
        private readonly IModel OrderPaidExchange;
    }
}
