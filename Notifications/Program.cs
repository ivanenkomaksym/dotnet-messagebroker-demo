using MassTransit;
using Notifications;
using Notifications.Consumers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<NotificationsWorker>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<CustomerCreatedConsumer>();

            x.UsingRabbitMq((context, cfg) => { cfg.ConfigureEndpoints(context); });
        });
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        config.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false);
        config.AddJsonFile("appsettings.k8s.json", optional: true, reloadOnChange: false);
    })
    .Build();

host.Run();
