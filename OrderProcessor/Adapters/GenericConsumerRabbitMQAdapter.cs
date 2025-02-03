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
        private readonly ConnectionStrings _connectionStrings;
        private readonly ILogger<GenericConsumerRabbitMQAdapter<TConsumer, TMessage>> _logger;
        private IConnection? _connection;
        private IModel? _channel;
        private string ExchangeName = $"{typeof(TMessage).Name}_RabbitMQAdapter";
        private readonly int MaxRetries = 10;
        private readonly int DelayInMilliseconds = 1000;

        public GenericConsumerRabbitMQAdapter(TConsumer consumer,
                                              IOptions<ConnectionStrings> rabbitMQOptions,
                                              ILogger<GenericConsumerRabbitMQAdapter<TConsumer, TMessage>> logger)
        {
            _consumer = consumer;
            _connectionStrings = rabbitMQOptions.Value;
            _logger = logger;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri($"{_connectionStrings.AMQPConnectionString}");

            // create connection  
            _connection = CreateConnectionWithRetry(factory);

            // create channel  
            _channel = _connection?.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);
        }

        private IConnection? CreateConnectionWithRetry(ConnectionFactory factory)
        {
            IConnection? connection = null;
            int attempt = 0;
            while (connection == null && attempt < MaxRetries)
            {
                try
                {
                    connection = factory.CreateConnection();
                    _logger.LogInformation("Connection established");
                }
                catch (Exception ex)
                {
                    attempt++;
                    _logger.LogInformation($"Attempt {attempt} failed: {ex.Message}");
                    if (attempt > MaxRetries)
                    {
                        _logger.LogError("Max retry attempts reached. Throwing exception");
                        throw;
                    }

                    _logger.LogInformation($"Waiting {DelayInMilliseconds}ms before retrying...");
                    Thread.Sleep(DelayInMilliseconds);
                }
            }

            return connection;
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
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}