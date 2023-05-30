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
            x.AddConsumer<OrderCreatedConsumer>();
            x.AddConsumer<PaymentResultConsumer>();

            x.UsingRabbitMq((context, cfg) => { cfg.ConfigureEndpoints(context); });
        });
    }).Build();

host.Run();
