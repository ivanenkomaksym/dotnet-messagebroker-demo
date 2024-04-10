using Common.Events;
using Common.Events.UserNotifications;
using MassTransit;
using System.Text.Json;

namespace Notifications.Consumers
{
    internal class PaymentResultConsumer : IConsumer<PaymentResult>
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly ILogger<PaymentResultConsumer> _logger;

        public PaymentResultConsumer(ISendEndpointProvider sendEndpointProvider, ILogger<PaymentResultConsumer> logger)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentResult> context)
        {
            // In
            var paymentResult = context.Message;
            ArgumentNullException.ThrowIfNull(paymentResult);
            var message = JsonSerializer.Serialize(paymentResult);
            _logger.LogInformation($"Received `PaymentResult` event with content: {message}");

            // Out
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{paymentResult.CustomerInfo.CustomerId.ToString()}"));

            var userPaymentResultEvent = new UserPaymentResult
            {
                CustomerId = paymentResult.CustomerInfo.CustomerId,
                OrderId = paymentResult.OrderId,
                PaymentStatus = paymentResult.PaymentStatus
            };

            await endpoint.Send(userPaymentResultEvent);

            message = JsonSerializer.Serialize(userPaymentResultEvent);
            _logger.LogInformation($"Sent `UserPaymentResult` event with content: {message}");
        }
    }
}
