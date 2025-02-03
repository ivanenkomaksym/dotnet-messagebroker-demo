using System.Text.Json;
using Common.Events;
using Common.Models.Shipment;
using MassTransit;
using Shipment.Repositories;

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

            await ProduceRemoveReserve(shipOrder, RemoveReserveReason.TakeFromStockForShipment);

            var deliveryStatus = DeliveryStatus.None;
            var delivery = await _shipmentRepository.CreateDelivery(new Delivery
            {
                Id = Guid.NewGuid(),
                OrderId = shipOrder.OrderId,
                CustomerInfo = shipOrder.CustomerInfo,
                ShippingAddress = shipOrder.ShippingAddress,
                DeliveryStatus = deliveryStatus
            });

            // Out
            _ = ScheduleShipmentUpdate(shipOrder, deliveryStatus);
        }

        private Task ScheduleShipmentUpdate(ShipOrder shipOrder, DeliveryStatus currentDeliveryStatus)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(10 * 1000); // wait 10s

                var newDeliveryStatus = currentDeliveryStatus;
                switch (currentDeliveryStatus)
                {
                    case DeliveryStatus.None:
                        newDeliveryStatus = DeliveryStatus.Shipping;
                        break;
                    case DeliveryStatus.Shipping:
                        newDeliveryStatus = DeliveryStatus.Shipped;
                        break;
                    case DeliveryStatus.Shipped:
                        newDeliveryStatus = DeliveryStatus.Collecting;
                        break;
                    default:
                        return;
                }

                var delivery = await _shipmentRepository.GetDeliveryByOrderId(shipOrder.OrderId);
                delivery.DeliveryStatus = newDeliveryStatus;
                var result = await _shipmentRepository.UpdateDelivery(delivery);

                await ProduceShipmentResult(shipOrder, newDeliveryStatus);

                _ = ScheduleShipmentUpdate(shipOrder, newDeliveryStatus);
            });
        }

        private async Task ProduceRemoveReserve(ShipOrder shipOrder, RemoveReserveReason reason)
        {
            var removeReserveEvent = new RemoveReserve
            {
                OrderId = shipOrder.OrderId,
                Reason = reason
            };

            await _publishEndpoint.Publish(removeReserveEvent);

            var message = JsonSerializer.Serialize(removeReserveEvent);
            _logger.LogInformation($"Sent `RemoveReserve` event with content: {message}");
        }

        private async Task ProduceShipmentResult(ShipOrder shipOrder, DeliveryStatus deliveryStatus)
        {
            var shipmentResultEvent = new ShipmentResult
            {
                OrderId = shipOrder.OrderId,
                CustomerInfo = shipOrder.CustomerInfo,
                DeliveryStatus = deliveryStatus
            };

            await _publishEndpoint.Publish(shipmentResultEvent);

            var message = JsonSerializer.Serialize(shipmentResultEvent);
            _logger.LogInformation($"Sent `ShipmentResult` event with content: {message}");
        }
    }
}