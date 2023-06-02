using System.Text.Json;
using Common.Events;
using OrderProcessor.Clients;
using MassTransit;

namespace OrderProcessor.Consumers
{
    internal class CancelOrderConsumer : IConsumer<CancelOrder>
    {
        private readonly IGrpcOrderClient _grpcOrderClient;
        private readonly ILogger<CancelOrderConsumer> _logger;

        public CancelOrderConsumer(IGrpcOrderClient grpcOrderClient, ILogger<CancelOrderConsumer> logger)
        {
            _grpcOrderClient = grpcOrderClient;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CancelOrder> context)
        {
            // In
            var cancelOrder = context.Message;
            var message = JsonSerializer.Serialize(cancelOrder);
            _logger.LogInformation($"Received `CancelOrder` event with content: {message}");

            // Out
            var result = await _grpcOrderClient.UpdateOrder(cancelOrder.OrderId, orderStatus: Common.Models.OrderStatus.Cancelled);
        }
    }
}
