using Common.Events;
using Common.Models;
using MassTransit;
using OrderProcessor.Clients;
using System.Text.Json;

namespace OrderProcessor.Consumers
{
    internal class PaymentResultConsumer : IConsumer<PaymentResult>
    {
        private readonly IGrpcOrderClient _grpcOrderClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<PaymentResultConsumer> _logger;

        public PaymentResultConsumer(IGrpcOrderClient grpcOrderClient, IPublishEndpoint publishEndpoint, ILogger<PaymentResultConsumer> logger)
        {
            _grpcOrderClient = grpcOrderClient;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentResult> context)
        {
            // In
            var paymentResult = context.Message;
            var message = JsonSerializer.Serialize(paymentResult);
            _logger.LogInformation($"Received `PaymentResult` event with content: {message}");

            // Out
            var order = await _grpcOrderClient.GetOrder(paymentResult.OrderId);
            var result = await _grpcOrderClient.UpdateOrder(paymentResult.OrderId, paymentStatus: paymentResult.PaymentStatus);

            switch (paymentResult.PaymentStatus)
            {
                case Common.Models.Payment.PaymentStatus.Failed:
                    // Payment failed, update order status
                    result = await _grpcOrderClient.UpdateOrder(paymentResult.OrderId, orderStatus: OrderStatus.PaymentFailed);
                    await ProduceRemoveReserve(paymentResult.OrderId, RemoveReserveReason.PaymentFailed);
                    break;
                case Common.Models.Payment.PaymentStatus.Expired:
                    // Payment failed, update order status
                    result = await _grpcOrderClient.UpdateOrder(paymentResult.OrderId, orderStatus: OrderStatus.PaymentFailed);
                    await ProduceRemoveReserve(paymentResult.OrderId, RemoveReserveReason.PaymentExpired);
                    break;
                case Common.Models.Payment.PaymentStatus.Paid:
                    // Payment succeded, update order status
                    result = await _grpcOrderClient.UpdateOrder(paymentResult.OrderId, orderStatus: OrderStatus.Paid);
                    await ProduceShipOrder(order);
                    // Shipment scheduled, update order status
                    result = await _grpcOrderClient.UpdateOrder(paymentResult.OrderId, orderStatus: OrderStatus.AwaitingShipment);
                    break;
                default:
                    break;
            }
        }

        private async Task ProduceRemoveReserve(Guid orderId, RemoveReserveReason reason)
        {
            var removeReserveEvent = new RemoveReserve
            {
                OrderId = orderId,
                Reason = reason
            };

            await _publishEndpoint.Publish(removeReserveEvent);

            var message = JsonSerializer.Serialize(removeReserveEvent);
            _logger.LogInformation($"Sent `RemoveReserve` event with content: {message}");
        }

        private async Task ProduceShipOrder(Order order)
        {
            var shipOrderEvent = new ShipOrder
            {
                OrderId = order.Id,
                CustomerInfo = order.CustomerInfo,
                ShippingAddress = order.ShippingAddress
            };

            await _publishEndpoint.Publish(shipOrderEvent);

            var message = JsonSerializer.Serialize(shipOrderEvent);
            _logger.LogInformation($"Sent `ShipOrder` event with content: {message}");
        }
    }
}
