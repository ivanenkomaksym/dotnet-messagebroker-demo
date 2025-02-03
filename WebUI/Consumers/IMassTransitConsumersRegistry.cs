namespace WebUI.Consumers
{
    public interface IMassTransitConsumersRegistry
    {
        public Task StartListeningForApplicationUser(Guid customerId);

        public Task StopListeningForApplicationUser(Guid customerId);
    }
}