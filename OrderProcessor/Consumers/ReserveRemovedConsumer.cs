using Common.Events;
using OrderProcessor.Services;
using MassTransit;
using System.Text.Json;

namespace OrderProcessor.Consumers
{
    internal class ReserveRemovedConsumer : IConsumer<ReserveRemoved>
    {
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public ReserveRemovedConsumer(IOrderService orderService, IPublishEndpoint publishEndpoint, ILogger<OrderCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReserveRemoved> context)
        {
            // In
            var reserveRemoved = context.Message;
            var message = JsonSerializer.Serialize(reserveRemoved);
            _logger.LogInformation($"Received `ReserveRemoved` event with content: {message}");
        }
    }
}
