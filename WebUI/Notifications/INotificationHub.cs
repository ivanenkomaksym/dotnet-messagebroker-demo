using Common.Events.UserNotifications;

namespace WebUI.Notifications
{
    public interface INotificationHub
    {
        public Task SendPaymentResultNotification(Guid customerId, UserPaymentResult userPaymentResult);
    }
}