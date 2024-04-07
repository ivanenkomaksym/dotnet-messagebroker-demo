using System.Diagnostics;
using System.Text.Json;
using Common.Configuration;
using Microsoft.Extensions.Options;
using OrderProcessor.Consumers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderProcessor.Adapters
{
    /// <summary>
    /// This is a RabbitMQ specific adapter for MassTransit consumers.
    /// It will handle messages sent directly from RabbitMQ brokers other than MassTransit.
    /// </summary>
    internal class GenericConsumerRabbitMQAdapter<TConsumer, TMessage> : BackgroundService
        where TMessage : class
        where TConsumer : BaseConsumer<TMessage>
    {
        private readonly TConsumer _consumer;
        private readonly RabbitMQOptions _rabbitMQOptions;
        private readonly ILogger<GenericConsumerRabbitMQAdapter<TConsumer, TMessage>> _logger;
        private IConnection _connection;
        private IModel _channel;
        private string ExchangeName = $"{typeof(TMessage).Name}_RabbitMQAdapter";

        public GenericConsumerRabbitMQAdapter(TConsumer consumer,
                                              IOptions<RabbitMQOptions> rabbitMQOptions,
                                              ILogger<GenericConsumerRabbitMQAdapter<TConsumer, TMessage>> logger)
        {
            _consumer = consumer;
            _rabbitMQOptions = rabbitMQOptions.Value;
            _logger = logger;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory 
            { 
                HostName = _rabbitMQOptions.HostName,
                Port = _rabbitMQOptions.Port
            };

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
            var message = JsonSerializer.Deserialize<TMessage>(content);
            Debug.Assert(message != null);

            await _consumer.HandleMessage(message);
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
