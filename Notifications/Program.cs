using Notifications;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<NotificationsWorker>();
    })
    .Build();

host.Run();
