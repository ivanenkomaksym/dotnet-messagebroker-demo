using System.Text;
using Common;
using Common.Persistence;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderProcessor
{
    public sealed class OrderProcessor
    {
        public OrderProcessor(IRabbitMQChannelRegistry rabbitMQChannelRegistry, string hostName)
        {
            RabbitMQChannelRegistry = rabbitMQChannelRegistry;
            HostName = hostName;

            NewOrderChannel = RabbitMQChannelRegistry.GetOrCreate(HostName, Consts.NewOrderQueue, (model, ea) => { NewOrderRequested(ea); });
        }

        void NewOrderRequested(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {message}");

            // TODO: OrderProcessor redirects client to pay for the order and after that notifies consumers

            NotifyConsumersAboutOrderPaid();
        }

        void NotifyConsumersAboutOrderPaid()
        {
            var orderPaidChannel = RabbitMQChannelRegistry.GetOrCreate(HostName, Consts.NewOrderQueue, null);

            const string message = "Order paid";
            var body = Encoding.UTF8.GetBytes(message);

            orderPaidChannel.BasicPublish(exchange: string.Empty,
                                          routingKey: Consts.OrderPaidQueue,
                                          basicProperties: null,
                                          body: body);

            Console.WriteLine($" [x] Sent {message}");
        }

        IRabbitMQChannelRegistry RabbitMQChannelRegistry;
        private readonly IModel NewOrderChannel;
        private readonly string HostName;
    }
}