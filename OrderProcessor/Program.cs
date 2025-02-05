using Common.Configuration;
using Common.Events;
using Common.Extensions;
using Common.Protos;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OrderProcessor;
using System.Net.Mime;
using OrderProcessor.Adapters;
using OrderProcessor.Clients;
using OrderProcessor.Consumers;

var hostBuilder = Host.CreateDefaultBuilder(args);

var host = hostBuilder
    .ConfigureLogging((hostContext,logging) =>
    {
        logging.AddLogging(hostContext.Configuration);
    })
    .ConfigureServices((hostContext, services)=>
    {
        hostBuilder.AddServiceDefaults(hostContext, services);
        services.AddHostedService<OrderProcessorWorker>();

        services.Configure<ConnectionStrings>(hostContext.Configuration.GetSection(ConnectionStrings.Name));

        var orderGrpcUrl = hostContext.Configuration.GetSection(ConnectionStrings.Name).GetValue<string>(nameof(ConnectionStrings.OrderGrpcUrl));
        ArgumentNullException.ThrowIfNull(orderGrpcUrl);

        services.AddSingleton<IGrpcOrderClient, GrpcOrderClient>()
            .AddGrpcServiceReference<OrderService.OrderServiceClient>(orderGrpcUrl, failureStatus: HealthStatus.Degraded);

        services.AddHostedService<GenericConsumerRabbitMQAdapter<OrderCreatedConsumer, OrderCreated>>();
        services.AddHostedService<GenericConsumerRabbitMQAdapter<CancelOrderConsumer, CancelOrder>>();
        services.AddHostedService<GenericConsumerRabbitMQAdapter<OrderCollectedConsumer, OrderCollected>>();
        services.AddHostedService<GenericConsumerRabbitMQAdapter<OrderUpdatedConsumer, OrderUpdated>>();
        services.AddHostedService<GenericConsumerRabbitMQAdapter<ReturnOrderConsumer, ReturnOrder>>();

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
                Extensions.ConfigureRabbitMq(context, cfg);

                // Explicitly configure endpoints so that these messages can be consumed by many consumers at the same time
                // https://masstransit.io/documentation/configuration#receive-endpoints
                cfg.ReceiveEndpoint(hostContext.HostingEnvironment.ApplicationName, e =>
                {
                    // Consuming messages from other systems where messages may not be produced by MassTransit, raw JSON is commonly used.
                    e.DefaultContentType = new ContentType("application/json");
                    e.UseRawJsonDeserializer();

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