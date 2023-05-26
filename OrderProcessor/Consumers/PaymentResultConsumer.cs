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
        }
    }
}
