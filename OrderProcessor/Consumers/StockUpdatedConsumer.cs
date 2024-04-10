using Common.Events;
using MassTransit;
using System.Text.Json;

namespace OrderProcessor.Consumers
{
    internal class StockUpdatedConsumer : IConsumer<StockUpdated>
    {
        private readonly ILogger<StockUpdatedConsumer> _logger;

        public StockUpdatedConsumer(IPublishEndpoint publishEndpoint, ILogger<StockUpdatedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<StockUpdated> context)
        {
            // In
            var stockUpdated = context.Message;
            var message = JsonSerializer.Serialize(stockUpdated);
            _logger.LogInformation($"Received `StockUpdated` event with content: {message}");

            return Task.CompletedTask;
        }
    }
}
