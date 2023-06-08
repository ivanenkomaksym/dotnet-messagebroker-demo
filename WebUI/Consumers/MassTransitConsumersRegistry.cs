using MassTransit;

namespace WebUI.Consumers
{
    public class MassTransitConsumersRegistry : IMassTransitConsumersRegistry
    {
        private readonly IReceiveEndpointConnector _receiveEndpointConnector;
        private readonly Dictionary<Guid, HostReceiveEndpointHandle> _hostReceiveEndpoints = new();

        public MassTransitConsumersRegistry(IReceiveEndpointConnector receiveEndpointConnector)
        {
            _receiveEndpointConnector = receiveEndpointConnector;
        }

        public async Task StartListeningForApplicationUser(Guid customerId)
        {
            if (_hostReceiveEndpoints.ContainsKey(customerId))
                return;

            _hostReceiveEndpoints.Add(customerId, _receiveEndpointConnector.ConnectReceiveEndpoint(customerId.ToString(), (context, cfg) =>
            {
                cfg.ConfigureConsumer<UserPaymentResultConsumer>(context);
            }));

            // wait for the receive endpoint to be ready, throws an exception if a fault occurs
            var ready = await _hostReceiveEndpoints[customerId].Ready;
        }

        public async Task StopListeningForApplicationUser(Guid customerId)
        {
            var result = _hostReceiveEndpoints.TryGetValue(customerId, out var handle);
            if (result)
            {
                await handle.StopAsync();
            }
        }
    }
}
