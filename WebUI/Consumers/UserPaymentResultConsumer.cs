using System.Text.Json;
using Common.Events.UserNotifications;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using WebUI.Notifications;

namespace WebUI.Consumers
{
    internal class UserPaymentResultConsumer : IConsumer<UserPaymentResult>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<UserPaymentResultConsumer> _logger;

        public UserPaymentResultConsumer(IHubContext<NotificationHub> hubContext, ILogger<UserPaymentResultConsumer> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserPaymentResult> context)
        {
            // In
            var userPaymentResult = context.Message;
            var message = JsonSerializer.Serialize(userPaymentResult);
            _logger.LogInformation($"Received `UserPaymentResult` event with content: {message}");

            // Out
            await _hubContext.Clients.Group(userPaymentResult.CustomerId.ToString()).SendAsync("ReceivePaymentResultNotification", userPaymentResult.OrderId.ToString(), userPaymentResult.PaymentStatus);
        }
    }
}