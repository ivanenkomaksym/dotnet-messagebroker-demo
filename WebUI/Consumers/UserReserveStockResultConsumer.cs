using Common.Events.UserNotifications;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using WebUI.Notifications;

namespace WebUI.Consumers
{
    internal class UserReserveStockResultConsumer : IConsumer<UserReserveStockResult>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<UserReserveStockResultConsumer> _logger;

        public UserReserveStockResultConsumer(IHubContext<NotificationHub> hubContext, ILogger<UserReserveStockResultConsumer> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserReserveStockResult> context)
        {
            // In
            var userReserveStockResult = context.Message;
            var message = JsonSerializer.Serialize(userReserveStockResult);
            _logger.LogInformation($"Received `UserReserveStockResult` event with content: {message}");

            // Out
            var failedProductNames = userReserveStockResult.FailedToReserveProducts?.Select(x => x.ProductName).ToList();
            await _hubContext.Clients.Group(userReserveStockResult.CustomerId.ToString()).SendAsync("ReceiveReserveStockResultNotification",
                                                                                                     userReserveStockResult.OrderId.ToString(),
                                                                                                     userReserveStockResult.ReserveResult,
                                                                                                     failedProductNames);
        }
    }
}