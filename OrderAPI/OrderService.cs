using System.Text;
using System.Text.Json;
using Common;
using Common.Configuration;
using Common.Models;
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
            var channel = RabbitMQChannelRegistry.GetOrCreateQueue(HostName, Port, Consts.OrderQueue, null);

            var message = JsonSerializer.Serialize(order);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: Consts.OrderQueue,
                                 basicProperties: null,
                                 body: body);

            Logger.LogInformation($"['{Consts.OrderQueue}' queue] Sent '{message}'");

            return Task.CompletedTask;
        }

        IRabbitMQChannelRegistry RabbitMQChannelRegistry;
        private readonly ILogger<OrderService> Logger;
        private readonly string HostName;
        private readonly ushort Port;
    }
}
