using System.Text;
using System.Text.Json;
using Common;
using Common.Configuration;
using Common.Persistence;
using RabbitMQ.Client;

namespace OrderAPI
{
    public class OrderService : IOrderService
    {
        public OrderService(IRabbitMQChannelRegistry rabbitMQChannelRegistry, IConfiguration configuration, ILogger<OrderService> logger)
        {
            RabbitMQChannelRegistry = rabbitMQChannelRegistry;

            var rabbitMQOptions = configuration.GetSection(RabbitMQOptions.Name).Get<RabbitMQOptions>();
            HostName = rabbitMQOptions.HostName;
            Port = rabbitMQOptions.Port;
            Logger = logger;
        }

        public Task CreateOrder(Order order)
        {
            var channel = RabbitMQChannelRegistry.GetOrCreateQueue(HostName, Port, Consts.NewOrderQueue, null);

            var message = $"Order \'{JsonSerializer.Serialize(order)}\' requested";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: Consts.NewOrderQueue,
                                 basicProperties: null,
                                 body: body);

            Logger.LogInformation($"[x] Sent {message}");

            return Task.CompletedTask;
        }

        IRabbitMQChannelRegistry RabbitMQChannelRegistry;
        private readonly ILogger<OrderService> Logger;
        private readonly string HostName;
        private readonly ushort Port;
    }
}
