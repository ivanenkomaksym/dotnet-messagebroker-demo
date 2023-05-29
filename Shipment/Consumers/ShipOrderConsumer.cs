using Common.Events;
using Common.Models.Shipment;
using MassTransit;
using Shipment.Repositories;
using System.Text.Json;

namespace Shipment.Consumers
{
    internal class ShipOrderConsumer : IConsumer<ShipOrder>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly ILogger<ShipOrderConsumer> _logger;

        public ShipOrderConsumer(IPublishEndpoint publishEndpoint, IShipmentRepository shipmentRepository, ILogger<ShipOrderConsumer> logger)
        {
            _publishEndpoint = publishEndpoint;
            _shipmentRepository = shipmentRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ShipOrder> context)
        {
            // In
            var shipOrder = context.Message;
            var message = JsonSerializer.Serialize(shipOrder);
            _logger.LogInformation($"Received `ShipOrder` event with content: {message}");

            await ProduceRemoveReserve(shipOrder.OrderId, RemoveReserveReason.TakeFromStockForShipment);

            var delivery = await _shipmentRepository.CreateDelivery(new Delivery
            {
                Id = Guid.NewGuid(),
                OrderId = shipOrder.OrderId,
                CustomerInfo = shipOrder.CustomerInfo,
                ShippingAddress = shipOrder.ShippingAddress,
                DeliveryStatus = DeliveryStatus.Shipping
            });

            // Out
            // TODO: Send ShipmentResult message
        }


        private async Task ProduceRemoveReserve(Guid orderId, RemoveReserveReason reason)
        {
            var removeReserveEvent = new RemoveReserve
            {
                OrderId = orderId,
                Reason = reason
            };

            await _publishEndpoint.Publish(removeReserveEvent);

            var message = JsonSerializer.Serialize(removeReserveEvent);
            _logger.LogInformation($"Sent `RemoveReserve` event with content: {message}");
        }
    }
}
