using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.Persistence
{
    public class StubRabbitMQChannelRegistry : IRabbitMQChannelRegistry
    {
        public IModel GetOrCreate(string hostname, string queue, EventHandler<BasicDeliverEventArgs> handler)
        {
            return new StubModel();
        }
    }
}
