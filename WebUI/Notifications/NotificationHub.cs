using Common.Models.Payment;
using Microsoft.AspNetCore.SignalR;

namespace WebUI.Notifications
{
    public class NotificationHub : Hub
    {
        public async override Task OnConnectedAsync()
        {
            string? customerId = Context.GetHttpContext()?.Request.Query["customerId"];
            if (customerId != null)
                // Associate the user with the connection using Groups.AddToGroupAsync
                await Groups.AddToGroupAsync(Context.ConnectionId, customerId);

            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            string? customerId = Context.GetHttpContext()?.Request.Query["customerId"];
            if (customerId != null)
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, customerId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendOrderStatusNotification(string userId, string orderId, PaymentStatus paymentStatus)
        {
            // Send a notification to the specific user
            await Clients.User(userId).SendAsync("ReceivePaymentResultNotification", orderId, paymentStatus);
        }
    }
}