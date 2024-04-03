using System.Diagnostics;
using System.Text.Json;
using Common.Events;
using MassTransit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderProcessor.Adapters
{
    /// <summary>
    /// This is a RabbitMQ specific adapter for <seealso cref="Consumers.OrderCreatedConsumer"/>
    /// It will handle messages sent directly from RabbitMQ brokers other than MassTransit.
    /// </summary>
    internal class OrderCreatedConsumerRabbitMQAdapter : BackgroundService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderCreatedConsumerRabbitMQAdapter> _logger;
        private IConnection _connection;
        private IModel _channel;
        private const string ExchangeName = $"{nameof(OrderCreated)}_RabbitMQAdapter";

        public OrderCreatedConsumerRabbitMQAdapter(IPublishEndpoint publishEndpoint, ILogger<OrderCreatedConsumerRabbitMQAdapter> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            // create connection  
            _connection = factory.CreateConnection();

            // create channel  
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);
        }

        protected override Task ExecuteAsync(CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();

            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName,
                               exchange: ExchangeName,
                               routingKey: string.Empty);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                // received message  
                var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());

                // handle the received message  
                await HandleMessage(content);
            };

            _channel.BasicConsume(queueName, true, consumer);
            return Task.CompletedTask;
        }

        private async Task HandleMessage(string content)
        {
            _logger.LogInformation($"Received `{ExchangeName}` event with content: {content}");

            // In
            var orderCreated = JsonSerializer.Deserialize<OrderCreated>(content);
            Debug.Assert(orderCreated != null);

            // Out
            var reserveStockEvent = new ReserveStock
            {
                OrderId = orderCreated.OrderId,
                CustomerInfo = orderCreated.CustomerInfo,
                Items = orderCreated.Items
            };

            await _publishEndpoint.Publish(reserveStockEvent);

            var message = JsonSerializer.Serialize(reserveStockEvent);
            _logger.LogInformation($"Sent `ReserveStock` event with content: {message}");
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
