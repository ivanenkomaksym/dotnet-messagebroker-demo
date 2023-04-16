using System.Text;
using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderProcessor
{
    public sealed class OrderProcessor
    {
        public OrderProcessor()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            Connection = factory.CreateConnection();
            OrderPaidChannel = Connection.CreateModel();
            NewOrderChannel = Connection.CreateModel();

            NewOrderChannel.QueueDeclare(queue: Consts.NewOrderQueue,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

            Consumer = new EventingBasicConsumer(NewOrderChannel);
            Consumer.Received += (model, ea) =>
            {
                NewOrderRequested(ea);
            };

            NewOrderChannel.BasicConsume(queue: Consts.NewOrderQueue,
                                         autoAck: true,
                                         consumer: Consumer);
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
            OrderPaidChannel.QueueDeclare(queue: Consts.OrderPaidQueue,
                                          durable: false,
                                          exclusive: false,
                                          autoDelete: false,
                                          arguments: null);

            const string message = "Order paid";
            var body = Encoding.UTF8.GetBytes(message);

            OrderPaidChannel.BasicPublish(exchange: string.Empty,
                                          routingKey: Consts.OrderPaidQueue,
                                          basicProperties: null,
                                          body: body);
            Console.WriteLine($" [x] Sent {message}");
        }

        private readonly IConnection Connection;
        private readonly IModel NewOrderChannel;
        private readonly IModel OrderPaidChannel;
        private readonly EventingBasicConsumer Consumer;
    }
}