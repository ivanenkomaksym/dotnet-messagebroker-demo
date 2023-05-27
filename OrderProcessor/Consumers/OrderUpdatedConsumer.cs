using Common.Events;
using MassTransit;
using System.Text.Json;

namespace OrderProcessor.Consumers
{
    internal class OrderUpdatedConsumer : IConsumer<OrderUpdated>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderUpdatedConsumer(IPublishEndpoint publishEndpoint, ILogger<OrderCreatedConsumer> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderUpdated> context)
        {
            // In
            var orderUpdated = context.Message;
            var message = JsonSerializer.Serialize(orderUpdated);
            _logger.LogInformation($"Received `OrderUpdated` event with content: {message}");

            // Out
            var reserveStockEvent = new ReserveStock
            {
                OrderId = orderUpdated.OrderId,
                Items = orderUpdated.Items
            };

            await _publishEndpoint.Publish(reserveStockEvent);

            message = JsonSerializer.Serialize(reserveStockEvent);
            _logger.LogInformation($"Sent `ReserveStock` event with content: {message}");
        }
    }
}
