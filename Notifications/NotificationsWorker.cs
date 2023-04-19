using System.Text;
using Common;
using Common.Configuration;
using Common.Persistence;

namespace Notifications
{
    public sealed class NotificationsWorker : BackgroundService
    {
        private readonly ILogger<NotificationsWorker> _logger;

        public NotificationsWorker(IConfiguration configuration, ILogger<NotificationsWorker> logger)
        {
            _logger = logger;

            var rabbitMQOptions = configuration.GetSection(RabbitMQOptions.Name).Get<RabbitMQOptions>();

            IRabbitMQChannelRegistry rabbitMQChannelRegistry;
            if (rabbitMQOptions.UseStub)
                rabbitMQChannelRegistry = new StubRabbitMQChannelRegistry();
            else
                rabbitMQChannelRegistry = new RabbitMQChannelRegistry();

            rabbitMQChannelRegistry.GetOrCreate(rabbitMQOptions.HostName, rabbitMQOptions.Port, Consts.OrderPaidQueue, (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($" [x] Received {message}");
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
