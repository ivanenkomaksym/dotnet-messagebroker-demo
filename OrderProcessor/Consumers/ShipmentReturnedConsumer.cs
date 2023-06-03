using Common.Events;
using MassTransit;
using OrderProcessor.Clients;
using System.Text.Json;

namespace OrderProcessor.Consumers
{
    internal class ShipmentReturnedConsumer : IConsumer<ShipmentReturned>
    {
        private readonly IGrpcOrderClient _grpcOrderClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<PaymentResultConsumer> _logger;

        public ShipmentReturnedConsumer(IGrpcOrderClient grpcOrderClient, IPublishEndpoint publishEndpoint, ILogger<PaymentResultConsumer> logger)
        {
            _grpcOrderClient = grpcOrderClient;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ShipmentReturned> context)
        {
            // In
            var shipmentReturned = context.Message;
            var message = JsonSerializer.Serialize(shipmentReturned);
            _logger.LogInformation($"Received `ShipmentReturned` event with content: {message}");

            // Out
            var result = await _grpcOrderClient.UpdateOrder(shipmentReturned.OrderId, orderStatus: Common.Models.OrderStatus.Refunding);

            var order = await _grpcOrderClient.GetOrder(shipmentReturned.OrderId);
            var refundPaymentEvent = new RefundPayment
            {
                OrderId = order.Id,
                CustomerInfo = order.CustomerInfo,
                PaymentInfo = order.PaymentInfo,
                TotalPrice = order.TotalPrice
            };

            await _publishEndpoint.Publish(refundPaymentEvent);

            message = JsonSerializer.Serialize(refundPaymentEvent);
            _logger.LogInformation($"Sent `RefundPayment` event with content: {message}");
        }
    }
}
