using MassTransit;
using Notifications;
using Notifications.Consumers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
     {
         services.AddHostedService<NotificationsWorker>();

         services.AddMassTransit(x =>
         {
             x.AddConsumer<PaymentResultConsumer>();
             x.AddConsumer<ShipmentResultConsumer>();

             x.UsingRabbitMq((context, cfg) =>
             {
                 // Explicitly configure endpoints so that these messages can be consumed by many consumers at the same time
                 // https://masstransit.io/documentation/configuration#receive-endpoints
                 cfg.ReceiveEndpoint(hostContext.HostingEnvironment.ApplicationName, e =>
                 {
                     e.ConfigureConsumer<PaymentResultConsumer>(context);
                     e.ConfigureConsumer<ShipmentResultConsumer>(context);
                 });
                 cfg.ConfigureEndpoints(context);
             });
         });
     }).Build();

host.Run();
