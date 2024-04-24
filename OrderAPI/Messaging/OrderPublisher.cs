using System.Text.Json;
using Common.Events;
using Common.Models;
using MassTransit;

namespace OrderAPI.Messaging
{
    internal class OrderPublisher : IOrderPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderPublisher> _logger;

        public OrderPublisher(IPublishEndpoint publishEndpoint, ILogger<OrderPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }
        public async Task CreateOrder(Order order)
        {
            var orderCreatedEvent = new OrderCreated
            {
                OrderId = order.Id,
                CustomerInfo = order.CustomerInfo,
                OrderStatus = order.OrderStatus,
                ShippingAddress = order.ShippingAddress,
                PaymentInfo = order.PaymentInfo,
                Items = order.Items,
                CreationDateTime = DateTime.Now
            };

            await _publishEndpoint.Publish(orderCreatedEvent);

            var message = JsonSerializer.Serialize(orderCreatedEvent);
            _logger.LogInformation($"Sent `OrderCreated` event with content: {message}");
        }

        public async Task UpdateOrder(Order order)
        {
            var orderUpdatedEvent = new OrderUpdated
            {
                OrderId = order.Id,
                PaymentStatus = Common.Models.Payment.PaymentStatus.Unpaid,
                CustomerInfo = order.CustomerInfo,
                OrderStatus = order.OrderStatus,
                ShippingAddress = order.ShippingAddress,
                PaymentInfo = order.PaymentInfo,
                Items = order.Items
            };

            await _publishEndpoint.Publish(orderUpdatedEvent);

            var message = JsonSerializer.Serialize(orderUpdatedEvent);
            _logger.LogInformation($"Sent `OrderUpdated` event with content: {message}");
        }

        public async Task CancelOrder(Guid orderId)
        {
            var cancelOrderEvent = new CancelOrder
            {
                OrderId = orderId,
                CancelDateTime = DateTime.Now
            };

            await _publishEndpoint.Publish(cancelOrderEvent);

            var message = JsonSerializer.Serialize(cancelOrderEvent);
            _logger.LogInformation($"Sent `CancelOrder` event with content: {message}");
        }

        public async Task OrderCollected(Guid orderId)
        {
            var orderCollectedEvent = new OrderCollected
            {
                OrderId = orderId,
                CollectedDateTime = DateTime.Now
            };

            await _publishEndpoint.Publish(orderCollectedEvent);

            var message = JsonSerializer.Serialize(orderCollectedEvent);
            _logger.LogInformation($"Sent `OrderCollected` event with content: {message}");
        }

        public async Task ReturnOrder(Guid orderId)
        {
            var returnOrderEvent = new ReturnOrder
            {
                OrderId = orderId,
                ReturnDateTime = DateTime.Now
            };

            await _publishEndpoint.Publish(returnOrderEvent);

            var message = JsonSerializer.Serialize(returnOrderEvent);
            _logger.LogInformation($"Sent `ReturnOrder` event with content: {message}");
        }
    }
}
