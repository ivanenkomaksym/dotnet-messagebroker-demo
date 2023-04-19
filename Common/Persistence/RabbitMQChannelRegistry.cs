using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.Persistence
{
    public class RabbitMQChannelRegistry : IRabbitMQChannelRegistry
    {
        public IModel GetOrCreate(string hostname, ushort port, string queue, EventHandler<BasicDeliverEventArgs> handler)
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

                Channels.Add(connection, channel);
            }

            return channel;
        }

        private readonly Dictionary<string, IConnection> Connections = new();
        private readonly Dictionary<IConnection, IModel> Channels = new();
    }
}
