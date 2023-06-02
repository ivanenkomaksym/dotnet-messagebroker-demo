using System.Text.Json;
using Common.Events;
using OrderProcessor.Clients;
using MassTransit;

namespace OrderProcessor.Consumers
{
    internal class OrderCollectedConsumer : IConsumer<OrderCollected>
    {
        private readonly IGrpcOrderClient _grpcOrderClient;
        private readonly ILogger<OrderCollectedConsumer> _logger;

        public OrderCollectedConsumer(IGrpcOrderClient grpcOrderClient, ILogger<OrderCollectedConsumer> logger)
        {
            _grpcOrderClient = grpcOrderClient;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCollected> context)
        {
            // In
            var orderCollected = context.Message;
            var message = JsonSerializer.Serialize(orderCollected);
            _logger.LogInformation($"Received `OrderCollected` event with content: {message}");

            // Out
            var result = await _grpcOrderClient.UpdateOrder(orderCollected.OrderId, orderStatus: Common.Models.OrderStatus.Completed);
        }
    }
}
