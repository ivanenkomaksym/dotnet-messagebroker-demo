using System.Text.Json;
using Common.Events;
using Common.Models;
using Common.Persistence;
using MassTransit;

namespace OrderAPI
{
    public class OrderService : IOrderService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IPublishEndpoint publishEndpoint, ILogger<OrderService> logger)
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

        IRabbitMQChannelRegistry RabbitMQChannelRegistry;
        private readonly ILogger<OrderService> Logger;
        private readonly string HostName;
        private readonly ushort Port;
    }
}
