using Common.Events;
using Common.Events.UserNotifications;
using MassTransit;
using System.Text.Json;

namespace Notifications.Consumers
{
    internal class PaymentRefundedConsumer : IConsumer<PaymentRefunded>
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly ILogger<PaymentRefundedConsumer> _logger;

        public PaymentRefundedConsumer(ISendEndpointProvider sendEndpointProvider, ILogger<PaymentRefundedConsumer> logger)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentRefunded> context)
        {
            // In
            var paymentRefunded = context.Message;
            var message = JsonSerializer.Serialize(paymentRefunded);
            _logger.LogInformation($"Received `PaymentRefunded` event with content: {message}");

            // Out
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{paymentRefunded.CustomerInfo.CustomerId.ToString()}"));

            var userPaymentResultEvent = new UserPaymentResult
            {
                CustomerId = paymentRefunded.CustomerInfo.CustomerId,
                OrderId = paymentRefunded.OrderId,
                PaymentStatus = paymentRefunded.PaymentStatus
            };

            await endpoint.Send(userPaymentResultEvent);

            message = JsonSerializer.Serialize(userPaymentResultEvent);
            _logger.LogInformation($"Sent `UserPaymentResult` event with content: {message}");
        }
    }
}
