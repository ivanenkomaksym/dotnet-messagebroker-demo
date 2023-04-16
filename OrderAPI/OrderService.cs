using System.Text;
using System.Text.Json;
using Common;
using RabbitMQ.Client;

namespace OrderAPI
{
    public class OrderService : IOrderService
    {
        public OrderService(ILogger<OrderService> logger)
        {
            Logger = logger;

            var factory = new ConnectionFactory { HostName = "localhost" };
            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();

            Channel.QueueDeclare(queue: Consts.NewOrderQueue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public Task CreateOrder(Order order)
        {
            var message = $"Order \'{JsonSerializer.Serialize(order)}\' requested";
            var body = Encoding.UTF8.GetBytes(message);

            Channel.BasicPublish(exchange: string.Empty,
                                 routingKey: Consts.NewOrderQueue,
                                 basicProperties: null,
                                 body: body);

            Logger.LogInformation($"[x] Sent {message}");

            return Task.CompletedTask;
        }

        private readonly ILogger<OrderService> Logger;
        private readonly IConnection Connection;
        private readonly IModel Channel; 
    }
}
