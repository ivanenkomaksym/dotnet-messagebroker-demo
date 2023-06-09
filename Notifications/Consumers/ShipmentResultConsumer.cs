using System.Text.Json;
using Common.Events;
using Common.Events.UserNotifications;
using MassTransit;

namespace Notifications.Consumers
{
    internal class ShipmentResultConsumer : IConsumer<ShipmentResult>
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly ILogger<PaymentResultConsumer> _logger;

        public ShipmentResultConsumer(ISendEndpointProvider sendEndpointProvider, ILogger<PaymentResultConsumer> logger)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ShipmentResult> context)
        {
            // In
            var shipmentResult = context.Message;
            var message = JsonSerializer.Serialize(shipmentResult);
            _logger.LogInformation($"Received `ShipmentResult` event with content: {message}");

            // Out
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{shipmentResult.CustomerInfo.CustomerId.ToString()}"));

            var userShipmentResultEvent = new UserShipmentResult
            {
                CustomerId = shipmentResult.CustomerInfo.CustomerId,
                OrderId = shipmentResult.OrderId,
                DeliveryStatus = shipmentResult.DeliveryStatus
            };

            await endpoint.Send(userShipmentResultEvent);

            message = JsonSerializer.Serialize(userShipmentResultEvent);
            _logger.LogInformation($"Sent `UserShipmentResult` event with content: {message}");
        }
    }
}
