using Common.Extensions;
using Shipment;
using Shipment.Data;
using Shipment.Repositories;
using Shipment.Consumers;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;

var hostBuilder = Host.CreateDefaultBuilder(args);
hostBuilder.ConfigureOpenTelemetry();

IHost host = hostBuilder
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();

        services.AddScoped<IShipmentContext, ShipmentContext>();
        services.AddScoped<IShipmentRepository, ShipmentRepository>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ShipOrderConsumer>();
            x.AddConsumer<ShipmentToBeReturnedConsumer>();

            x.UsingRabbitMq((context, cfg) => { cfg.ConfigureEndpoints(context); });
        });

        services.AddHealthChecks()
                .AddMongoDb(hostContext.Configuration["DatabaseSettings:ConnectionString"], "MongoDb Health", HealthStatus.Degraded);

        BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
    }).Build();

host.Run();
