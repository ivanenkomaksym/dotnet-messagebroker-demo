using Common.Configuration;
using Common.Persistence;
using NToastNotify;

namespace WebUI.Notifications
{
    public class NotificationClient:  INotificationClient
    {
        private readonly ILogger<NotificationClient> _logger;
        private readonly IRabbitMQChannelRegistry _rabbitMQChannelRegistry;
        private readonly RabbitMQOptions _rabbitMQOptions;
        private readonly IToastNotification _toastNotification;

        public NotificationClient(IConfiguration configuration, IToastNotification toastNotification, ILogger<NotificationClient> logger)
        {
            _logger = logger;
            _toastNotification = toastNotification;

            _rabbitMQOptions = configuration.GetSection(RabbitMQOptions.Name).Get<RabbitMQOptions>();

            if (_rabbitMQOptions.UseStub)
                _rabbitMQChannelRegistry = new StubRabbitMQChannelRegistry();
            else
                _rabbitMQChannelRegistry = new RabbitMQChannelRegistry();
        }
    }
}
