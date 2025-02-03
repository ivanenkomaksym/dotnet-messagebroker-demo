using System.Text.Json;
using Common.Events;
using MassTransit;
using OrderProcessor.Clients;

namespace OrderProcessor.Consumers
{
    internal class ReturnOrderConsumer : BaseConsumer<ReturnOrder>, IConsumer<ReturnOrder>
    {
        private readonly IGrpcOrderClient _grpcOrderClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<ReturnOrderConsumer> _logger;

        public ReturnOrderConsumer(IGrpcOrderClient grpcOrderClient, ILogger<ReturnOrderConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _grpcOrderClient = grpcOrderClient;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<ReturnOrder> context)
        {
            // In
            var returnOrder = context.Message;

            await HandleMessage(returnOrder);
        }

        public override async Task HandleMessage(ReturnOrder returnOrder)
        {
            var message = JsonSerializer.Serialize(returnOrder);
            _logger.LogInformation($"Received `ReturnOrder` event with content: {message}");

            // Out
            var order = await _grpcOrderClient.GetOrder(returnOrder.OrderId);
            var result = await _grpcOrderClient.UpdateOrder(returnOrder.OrderId, orderStatus: Common.Models.OrderStatus.AwaitingReturn);

            var shipmentToBeReturnedEvent = new ShipmentToBeReturned
            {
                OrderId = order.Id,
                CustomerInfo = order.CustomerInfo,
                ShippingAddress = order.ShippingAddress
            };

            await _publishEndpoint.Publish(shipmentToBeReturnedEvent);

            message = JsonSerializer.Serialize(shipmentToBeReturnedEvent);
            _logger.LogInformation($"Sent `ShipmentToBeReturned` event with content: {message}");
        }
    }
}