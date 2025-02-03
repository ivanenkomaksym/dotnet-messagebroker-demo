using System.Text.Json;
using Common.Events.UserNotifications;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using WebUI.Notifications;

namespace WebUI.Consumers
{
    internal class UserShipmentResultConsumer : IConsumer<UserShipmentResult>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<UserShipmentResultConsumer> _logger;

        public UserShipmentResultConsumer(IHubContext<NotificationHub> hubContext, ILogger<UserShipmentResultConsumer> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserShipmentResult> context)
        {
            // In
            var userShipmentResult = context.Message;
            var message = JsonSerializer.Serialize(userShipmentResult);
            _logger.LogInformation($"Received `UserShipmentResult` event with content: {message}");

            // Out
            await _hubContext.Clients.Group(userShipmentResult.CustomerId.ToString()).SendAsync("ReceiveShipmentResultNotification", userShipmentResult.OrderId.ToString(), userShipmentResult.DeliveryStatus);
        }
    }
}