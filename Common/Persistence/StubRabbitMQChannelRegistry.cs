﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.Persistence
{
    public class StubRabbitMQChannelRegistry : IRabbitMQChannelRegistry
    {
        public IModel GetOrCreateQueue(string hostname, ushort port, string queue, EventHandler<BasicDeliverEventArgs> handler)
        {
            return new StubModel();
        }
    }
}
