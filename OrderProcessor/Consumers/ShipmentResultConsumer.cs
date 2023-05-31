using System.Text.Json;
using Common.Events;
using MassTransit;
using OrderProcessor.Clients;

namespace OrderProcessor.Consumers
{
    internal class ShipmentResultConsumer : IConsumer<ShipmentResult>
    {
        private readonly IGrpcOrderClient _grpcOrderClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<PaymentResultConsumer> _logger;

        public ShipmentResultConsumer(IGrpcOrderClient grpcOrderClient, IPublishEndpoint publishEndpoint, ILogger<PaymentResultConsumer> logger)
        {
            _grpcOrderClient = grpcOrderClient;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ShipmentResult> context)
        {
            // In
            var shipmentResult = context.Message;
            var message = JsonSerializer.Serialize(shipmentResult);
            _logger.LogInformation($"Received `ShipmentResult` event with content: {message}");

            // Out
        }
    }
}
