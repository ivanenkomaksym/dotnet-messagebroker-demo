using System.Text.Json;
using Common.Events;
using Common.Events.UserNotifications;
using MassTransit;

namespace Notifications.Consumers
{
    internal sealed class ReserveStockResultConsumer : IConsumer<ReserveStockResult>
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        ILogger<ReserveStockResultConsumer> _logger;

        public ReserveStockResultConsumer(ISendEndpointProvider sendEndpointProvider, ILogger<ReserveStockResultConsumer> logger)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReserveStockResult> context)
        {
            // In
            var reserveStockResult = context.Message;
            ArgumentNullException.ThrowIfNull(reserveStockResult);
            ArgumentNullException.ThrowIfNull(reserveStockResult.CustomerInfo);

            var message = JsonSerializer.Serialize(reserveStockResult);
            _logger.LogInformation($"Received `ReserveStockResult` event with content: {message}");

            // Out
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{reserveStockResult.CustomerInfo.CustomerId}"));

            var userReserveStockResult = new UserReserveStockResult
            {
                CustomerId = reserveStockResult.CustomerInfo.CustomerId,
                OrderId = reserveStockResult.OrderId,
                ReserveResult = reserveStockResult.ReserveResult,
                FailedToReserveProducts = reserveStockResult.FailedToReserveProducts
            };

            await endpoint.Send(userReserveStockResult);

            message = JsonSerializer.Serialize(userReserveStockResult);
            _logger.LogInformation($"Sent `UserReserveStockResult` event with content: {message}");
        }
    }
}