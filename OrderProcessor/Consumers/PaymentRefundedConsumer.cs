using Common.Events;
using MassTransit;
using OrderProcessor.Clients;
using System.Text.Json;

namespace OrderProcessor.Consumers
{
    internal class PaymentRefundedConsumer : IConsumer<PaymentRefunded>
    {
        private readonly IGrpcOrderClient _grpcOrderClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<PaymentRefundedConsumer> _logger;

        public PaymentRefundedConsumer(IGrpcOrderClient grpcOrderClient, IPublishEndpoint publishEndpoint, ILogger<PaymentRefundedConsumer> logger)
        {
            _grpcOrderClient = grpcOrderClient;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentRefunded> context)
        {
            // In
            var paymentResult = context.Message;
            var message = JsonSerializer.Serialize(paymentResult);
            _logger.LogInformation($"Received `PaymentRefunded` event with content: {message}");

            // Out
            var order = await _grpcOrderClient.GetOrder(paymentResult.OrderId);
            var result = await _grpcOrderClient.UpdateOrder(paymentResult.OrderId, Common.Models.OrderStatus.Refunded);
        }
    }
}
