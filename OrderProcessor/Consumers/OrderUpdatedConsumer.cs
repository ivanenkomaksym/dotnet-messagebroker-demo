using Common.Events;
using Common.Models;
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
            switch (orderUpdated.PaymentStatus)
            {
                case Common.Models.Payment.PaymentStatus.Unpaid:
                case Common.Models.Payment.PaymentStatus.Failed:
                case Common.Models.Payment.PaymentStatus.Expired:
                    await ProduceReserveStock(orderUpdated.OrderId, orderUpdated.Items);
                    break;
                case Common.Models.Payment.PaymentStatus.Paid:
                case Common.Models.Payment.PaymentStatus.Refunding:
                case Common.Models.Payment.PaymentStatus.Refunded:
                default:
                    break;
            }
        }

        private async Task ProduceReserveStock(Guid orderId, IList<OrderItem> items)
        {
            var reserveStockEvent = new ReserveStock
            {
                OrderId = orderId,
                Items = items
            };

            await _publishEndpoint.Publish(reserveStockEvent);

            var message = JsonSerializer.Serialize(reserveStockEvent);
            _logger.LogInformation($"Sent `ReserveStock` event with content: {message}");
        }
    }
}
