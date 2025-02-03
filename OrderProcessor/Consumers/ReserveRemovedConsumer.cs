using System.Text.Json;
using Common.Events;
using MassTransit;

namespace OrderProcessor.Consumers
{
    internal class ReserveRemovedConsumer : IConsumer<ReserveRemoved>
    {
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public ReserveRemovedConsumer(IPublishEndpoint publishEndpoint, ILogger<OrderCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ReserveRemoved> context)
        {
            // In
            var reserveRemoved = context.Message;
            var message = JsonSerializer.Serialize(reserveRemoved);
            _logger.LogInformation($"Received `ReserveRemoved` event with content: {message}");

            return Task.CompletedTask;
        }
    }
}