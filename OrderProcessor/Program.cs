using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using OrderProcessor;
using OrderProcessor.Clients;
using OrderProcessor.Consumers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<OrderProcessorWorker>();

        services.AddSingleton<IGrpcOrderClient, GrpcOrderClient>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderCreatedConsumer>();
            x.AddConsumer<OrderUpdatedConsumer>();
            x.AddConsumer<ReserveStockResultConsumer>();
            x.AddConsumer<ReserveRemovedConsumer>();
            x.AddConsumer<PaymentResultConsumer>();
            x.AddConsumer<ShipmentResultConsumer>();
            x.AddConsumer<OrderCollectedConsumer>();
            x.AddConsumer<CancelOrderConsumer>();
            x.AddConsumer<ReturnOrderConsumer>();
            x.AddConsumer<ShipmentReturnedConsumer>();
            x.AddConsumer<PaymentRefundedConsumer>();
            x.AddConsumer<StockUpdatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                // Explicitly configure endpoints so that these messages can be consumed by many consumers at the same time
                // https://masstransit.io/documentation/configuration#receive-endpoints
                cfg.ReceiveEndpoint(hostContext.HostingEnvironment.ApplicationName, e =>
                {
                    e.ConfigureConsumer<OrderCreatedConsumer>(context);
                    e.ConfigureConsumer<OrderUpdatedConsumer>(context);
                    e.ConfigureConsumer<ReserveStockResultConsumer>(context);
                    e.ConfigureConsumer<ReserveRemovedConsumer>(context);
                    e.ConfigureConsumer<PaymentResultConsumer>(context);
                    e.ConfigureConsumer<ShipmentResultConsumer>(context);
                    e.ConfigureConsumer<OrderCollectedConsumer>(context);
                    e.ConfigureConsumer<CancelOrderConsumer>(context);
                    e.ConfigureConsumer<ReturnOrderConsumer>(context);
                    e.ConfigureConsumer<ShipmentReturnedConsumer>(context);
                    e.ConfigureConsumer<PaymentRefundedConsumer>(context);
                    e.ConfigureConsumer<StockUpdatedConsumer>(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .Build();

host.Run();
