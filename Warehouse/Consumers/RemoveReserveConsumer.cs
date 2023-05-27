using System.Text.Json;
using Common.Events;
using Warehouse.Repositories;
using MassTransit;
using System.Diagnostics;

namespace Warehouse.Consumers
{
    internal class RemoveReserveConsumer : IConsumer<RemoveReserve>
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<RemoveReserveConsumer> _logger;

        public RemoveReserveConsumer(IWarehouseRepository warehouseRepository, IPublishEndpoint publishEndpoint, ILogger<RemoveReserveConsumer> logger)
        {
            _warehouseRepository = warehouseRepository;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RemoveReserve> context)
        {
            // In
            var removeReserve = context.Message;
            var message = JsonSerializer.Serialize(removeReserve);
            _logger.LogInformation($"Received `RemoveReserve` event with content: {message}");

            var result = await _warehouseRepository.DeleteReserve(removeReserve.OrderId);
            Debug.Assert(result);

            // Out
            var reserveRemovedEvent = new ReserveRemoved
            {
                OrderId = removeReserve.OrderId,
            };

            await _publishEndpoint.Publish(reserveRemovedEvent);

            message = JsonSerializer.Serialize(reserveRemovedEvent);
            _logger.LogInformation($"Sent `ReserveRemoved` event with content: {message}");
        }
    }
}
