using System.Text.Json;
using Common.Events;
using MassTransit;

namespace OrderProcessor.Consumers
{
    internal class OrderCreatedConsumer : IConsumer<OrderCreated>
    {
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<OrderCreated> context)
        {
            var message = JsonSerializer.Serialize(context.Message);
            _logger.LogInformation($"Received `OrderCreated` event with content: {message}");

            return Task.CompletedTask;
        }
    }
}
