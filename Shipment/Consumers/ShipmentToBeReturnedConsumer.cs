using Common.Events;
using Common.Models.Shipment;
using MassTransit;
using Shipment.Repositories;
using System.Text.Json;

namespace Shipment.Consumers
{
    internal class ShipmentToBeReturnedConsumer : IConsumer<ShipmentToBeReturned>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly ILogger<ShipOrderConsumer> _logger;

        public ShipmentToBeReturnedConsumer(IPublishEndpoint publishEndpoint, IShipmentRepository shipmentRepository, ILogger<ShipOrderConsumer> logger)
        {
            _publishEndpoint = publishEndpoint;
            _shipmentRepository = shipmentRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ShipmentToBeReturned> context)
        {
            // In
            var shipmentToBeReturned = context.Message;
            var message = JsonSerializer.Serialize(shipmentToBeReturned);
            _logger.LogInformation($"Received `ShipmentToBeReturnedConsumer` event with content: {message}");


            var delivery = await _shipmentRepository.GetDeliveryByOrderId(shipmentToBeReturned.OrderId);
            delivery.DeliveryStatus = DeliveryStatus.Returning;
            var result = await _shipmentRepository.UpdateDelivery(delivery);

            _ = AwaitShipmentReturn(shipmentToBeReturned);
        }

        private Task AwaitShipmentReturn(ShipmentToBeReturned shipmentToBeReturned)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(10 * 1000); // wait 10s

                var delivery = await _shipmentRepository.GetDeliveryByOrderId(shipmentToBeReturned.OrderId);
                delivery.DeliveryStatus = DeliveryStatus.Returned;
                var result = await _shipmentRepository.UpdateDelivery(delivery);

                await ProduceShipmentReturned(shipmentToBeReturned, delivery.DeliveryStatus);
            });
        }

        private async Task ProduceShipmentReturned(ShipmentToBeReturned shipmentToBeReturned, DeliveryStatus deliveryStatus)
        {
            var shipmentReturnedEvent = new ShipmentReturned
            {
                OrderId = shipmentToBeReturned.OrderId,
                CustomerInfo = shipmentToBeReturned.CustomerInfo,
                DeliveryStatus = DeliveryStatus.Returned
            };

            await _publishEndpoint.Publish(shipmentReturnedEvent);

            var message = JsonSerializer.Serialize(shipmentReturnedEvent);
            _logger.LogInformation($"Sent `ShipmentReturned` event with content: {message}");
        }
    }
}
