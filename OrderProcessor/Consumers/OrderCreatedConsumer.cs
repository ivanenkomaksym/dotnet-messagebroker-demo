using System.Text.Json;
using Common.Events;
using MassTransit;

namespace OrderProcessor.Consumers
{
    internal class OrderCreatedConsumer : IConsumer<OrderCreated>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(IPublishEndpoint publishEndpoint, ILogger<OrderCreatedConsumer> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreated> context)
        {
            // In
            var orderCreated = context.Message;
            var message = JsonSerializer.Serialize(orderCreated);
            _logger.LogInformation($"Received `OrderCreated` event with content: {message}");

            // Out
            var reserveStockEvent = new ReserveStock
            {
                OrderId = orderCreated.OrderId,
                CustomerInfo = orderCreated.CustomerInfo,
                Items = orderCreated.Items
            };

            await _publishEndpoint.Publish(reserveStockEvent);

            message = JsonSerializer.Serialize(reserveStockEvent);
            _logger.LogInformation($"Sent `ReserveStock` event with content: {message}");
        }
    }
}
