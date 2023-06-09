using Common.Events;
using Common.Events.UserNotifications;
using MassTransit;
using System.Text.Json;

namespace Notifications.Consumers
{
    internal class ShipmentReturnedConsumer : IConsumer<ShipmentReturned>
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly ILogger<ShipmentReturnedConsumer> _logger;

        public ShipmentReturnedConsumer(ISendEndpointProvider sendEndpointProvider, ILogger<ShipmentReturnedConsumer> logger)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ShipmentReturned> context)
        {
            // In
            var shipmentReturned = context.Message;
            var message = JsonSerializer.Serialize(shipmentReturned);
            _logger.LogInformation($"Received `ShipmentReturned` event with content: {message}");

            // Out
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{shipmentReturned.CustomerInfo.CustomerId.ToString()}"));

            var userShipmentResultEvent = new UserShipmentResult
            {
                CustomerId = shipmentReturned.CustomerInfo.CustomerId,
                OrderId = shipmentReturned.OrderId,
                DeliveryStatus = shipmentReturned.DeliveryStatus
            };

            await endpoint.Send(userShipmentResultEvent);

            message = JsonSerializer.Serialize(userShipmentResultEvent);
            _logger.LogInformation($"Sent `UserShipmentResult` event with content: {message}");
        }
    }
}
