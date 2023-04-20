using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.Persistence
{
    public class RabbitMQChannelRegistry : IRabbitMQChannelRegistry
    {
        public IModel GetOrCreateChannel(string hostname, ushort port)
        {
            var foundConnection = Connections.TryGetValue(hostname, out var connection);
            if (!foundConnection)
            {
                var factory = new ConnectionFactory { HostName = hostname, Port = port };
                connection = factory.CreateConnection();

                Connections.Add(hostname, connection);
            }

            var foundChannel = Channels.TryGetValue(connection, out var channel);
            if (!foundChannel)
            {
                channel = connection.CreateModel();

                Channels.Add(connection, channel);
            }

            return channel;
        }

        public IModel GetOrCreateQueue(string hostname, ushort port, string queue, EventHandler<BasicDeliverEventArgs> handler)
        {
            var channel = GetOrCreateChannel(hostname, port);
            channel.QueueDeclare(queue: queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            if (handler != null)
            {

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += handler;

                channel.BasicConsume(queue: queue,
                                     autoAck: true,
                                     consumer: consumer);
            }

            return channel;
        }

        public IModel GetOrCreateExchange(string hostname, ushort port, string exchange, string bindingKey, EventHandler<BasicDeliverEventArgs> handler)
        {
            var channel = GetOrCreateChannel(hostname, port);
            channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic);

            if (handler != null)
            {
                // declare a server-named queue
                var queueName = channel.QueueDeclare().QueueName;

                channel.QueueBind(queue: queueName,
                                  exchange: exchange,
                                  routingKey: bindingKey);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += handler;

                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);
            }

            return channel;
        }

        private readonly Dictionary<string, IConnection> Connections = new();
        private readonly Dictionary<IConnection, IModel> Channels = new();
    }
}
