using Common.Events;
using MassTransit;
using OrderProcessor.Services;
using System.Text.Json;

namespace OrderProcessor.Consumers
{
    internal class PaymentResultConsumer : IConsumer<PaymentResult>
    {
        private readonly IOrderService _orderService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<PaymentResultConsumer> _logger;

        public PaymentResultConsumer(IOrderService orderService, IPublishEndpoint publishEndpoint, ILogger<PaymentResultConsumer> logger)
        {
            _orderService = orderService;
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
            var order = await _orderService.GetOrderById(paymentResult.OrderId);
            order.PaymentStatus = paymentResult.PaymentStatus;
            await _orderService.UpdateOrder(order);

            switch (paymentResult.PaymentStatus)
            {
                case Common.Models.Payment.PaymentStatus.Failed:
                    await ProduceRemoveReserve(paymentResult.OrderId, RemoveReserveReason.PaymentFailed);
                    break;
                case Common.Models.Payment.PaymentStatus.Expired:
                    await ProduceRemoveReserve(paymentResult.OrderId, RemoveReserveReason.PaymentExpired);
                    break;
                case Common.Models.Payment.PaymentStatus.Paid:
                    await ProduceShipOrder(paymentResult.OrderId);
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

        private Task ProduceShipOrder(Guid orderId)
        {
            // TODO: Implement shipping order
            return Task.CompletedTask;
        }
    }
}
