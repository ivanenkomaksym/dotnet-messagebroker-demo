using Common.Events;
using MassTransit;
using System.Text.Json;

namespace Notifications.Consumers
{
    internal class PaymentResultConsumer : IConsumer<PaymentResult>
    {
        private readonly IPublishEndpoint _publicEndpoint;
        private readonly ILogger<PaymentResultConsumer> _logger;

        public PaymentResultConsumer(IPublishEndpoint publishEndpoint, ILogger<PaymentResultConsumer> logger)
        {
            _publicEndpoint = publishEndpoint;
            _logger = logger;
        }

        public Task Consume(ConsumeContext<PaymentResult> context)
        {
            var message = JsonSerializer.Serialize(context.Message);
            _logger.LogInformation($"Received `PaymentResult` event with content: {message}");

            return Task.CompletedTask;
        }
    }
}
