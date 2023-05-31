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

            var deliveryStatus = DeliveryStatus.Shipping;
            var delivery = await _shipmentRepository.CreateDelivery(new Delivery
            {
                Id = Guid.NewGuid(),
                OrderId = shipOrder.OrderId,
                CustomerInfo = shipOrder.CustomerInfo,
                ShippingAddress = shipOrder.ShippingAddress,
                DeliveryStatus = deliveryStatus
            });

            // Out
            await ProduceShipmentResult(shipOrder.OrderId, deliveryStatus);

            _ = ScheduleShipmentUpdate(shipOrder.OrderId, deliveryStatus);
        }

        private Task ScheduleShipmentUpdate(Guid orderId, DeliveryStatus currentDeliveryStatus)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(30 * 1000); // wait 30s

                var newDeliveryStatus = currentDeliveryStatus;
                switch (currentDeliveryStatus)
                {
                    case DeliveryStatus.Shipping:
                        newDeliveryStatus = DeliveryStatus.Shipped;
                        break;
                    case DeliveryStatus.Shipped:
                        newDeliveryStatus = DeliveryStatus.Collecting;
                        break;
                    default:
                        return;
                }

                var delivery = await _shipmentRepository.GetDeliveryByOrderId(orderId);
                delivery.DeliveryStatus = newDeliveryStatus;
                var result = await _shipmentRepository.UpdateDelivery(delivery);

                await ProduceShipmentResult(orderId, newDeliveryStatus);

                _ = ScheduleShipmentUpdate(orderId, newDeliveryStatus);
            });
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

        private async Task ProduceShipmentResult(Guid orderId, DeliveryStatus deliveryStatus)
        {
            var shipmentResultEvent = new ShipmentResult
            {
                OrderId = orderId,
                DeliveryStatus = deliveryStatus
            };

            await _publishEndpoint.Publish(shipmentResultEvent);

            var message = JsonSerializer.Serialize(shipmentResultEvent);
            _logger.LogInformation($"Sent `ShipmentResult` event with content: {message}");
        }
    }
}
