using Common.Events;
using MassTransit;
using System.Text.Json;

namespace Warehouse.Consumers
{
    internal class ReserveStockConsumer : IConsumer<ReserveStock>
    {
        private readonly ILogger<ReserveStockConsumer> _logger;

        public ReserveStockConsumer(ILogger<ReserveStockConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ReserveStock> context)
        {
            var message = JsonSerializer.Serialize(context.Message);
            _logger.LogInformation($"Received `ReserveStock` event with content: {message}");

            return Task.CompletedTask;
        }
    }
}
