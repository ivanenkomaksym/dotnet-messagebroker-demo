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

            x.UsingRabbitMq((context, cfg) => { cfg.ConfigureEndpoints(context); });
        });
    })
    .Build();

host.Run();
