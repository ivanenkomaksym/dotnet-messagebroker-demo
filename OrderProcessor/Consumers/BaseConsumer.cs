namespace OrderProcessor.Consumers
{
    internal abstract class BaseConsumer<TMessage>
    {
        public abstract Task HandleMessage(TMessage orderCreated);
    }
}