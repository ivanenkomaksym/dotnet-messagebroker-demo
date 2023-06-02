using System.Text.Json;
using Common.Events;
using Common.Models;
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
            var result = await _grpcOrderClient.UpdateOrder(shipmentResult.OrderId, deliveryStatus: shipmentResult.DeliveryStatus);

            switch (shipmentResult.DeliveryStatus)
            {
                case Common.Models.Shipment.DeliveryStatus.Shipping:
                    // Order shipment in progress, update order status
                    result = await _grpcOrderClient.UpdateOrder(shipmentResult.OrderId, orderStatus: OrderStatus.Shipping);
                    break;
                case Common.Models.Shipment.DeliveryStatus.Shipped:
                    // Order shipped, update order status
                    result = await _grpcOrderClient.UpdateOrder(shipmentResult.OrderId, orderStatus: OrderStatus.AwaitingCollection);
                    break;
                default:
                    break;
            }
        }
    }
}
