using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.Persistence
{
    public interface IRabbitMQChannelRegistry
    {
        IModel GetOrCreateQueue(string hostname, ushort port, string queue, EventHandler<BasicDeliverEventArgs> handler);

        IModel GetOrCreateExchange(string hostname, ushort port, string exchange, string routingKey, EventHandler<BasicDeliverEventArgs> handler);
    }
}
