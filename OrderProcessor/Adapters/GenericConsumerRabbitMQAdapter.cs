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
        private IChannel? _channel;
        private readonly string ExchangeName = $"{typeof(TMessage).Name}_RabbitMQAdapter";
        private readonly int MaxRetries = 10;
        private readonly int DelayInMilliseconds = 1000;

        public GenericConsumerRabbitMQAdapter(TConsumer consumer,
                                              IOptions<ConnectionStrings> rabbitMQOptions,
                                              ILogger<GenericConsumerRabbitMQAdapter<TConsumer, TMessage>> logger)
        {
            _consumer = consumer;
            _connectionStrings = rabbitMQOptions.Value;
            _logger = logger;
        }

        private async Task InitRabbitMQ()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri($"{_connectionStrings.AMQPConnectionString}");

            // create connection  
            _connection = await CreateConnectionWithRetry(factory);

            // create channel  
            _channel = await _connection?.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Fanout);
        }

        private async Task<IConnection?> CreateConnectionWithRetry(ConnectionFactory factory)
        {
            IConnection? connection = null;
            int attempt = 0;
            while (connection == null && attempt < MaxRetries)
            {
                try
                {
                    connection = await factory.CreateConnectionAsync();
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

        protected async override Task ExecuteAsync(CancellationToken cancelToken)
        {
            await InitRabbitMQ();

            cancelToken.ThrowIfCancellationRequested();

            var queue = await _channel.QueueDeclareAsync();
            var queueName = queue.QueueName;
            await _channel.QueueBindAsync(queue: queueName,
                                          exchange: ExchangeName,
                                          routingKey: string.Empty);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (ch, ea) =>
            {
                // received message  
                var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());

                // handle the received message  
                await HandleMessage(content);
            };

            await _channel.BasicConsumeAsync(queueName, true, consumer);
        }

        private async Task HandleMessage(string content)
        {
            var message = JsonSerializer.Deserialize<TMessage>(content);
            Debug.Assert(message != null);

            await _consumer.HandleMessage(message);
        }

        public async override void Dispose()
        {
            await _channel?.CloseAsync();
            await _connection?.CloseAsync();
            base.Dispose();
        }
    }
}