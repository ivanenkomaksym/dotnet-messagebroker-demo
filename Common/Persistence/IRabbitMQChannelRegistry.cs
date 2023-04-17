using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.Persistence
{
    public interface IRabbitMQChannelRegistry
    {
        IModel GetOrCreate(string hostname, string queue, EventHandler<BasicDeliverEventArgs> handler);
    }
}
