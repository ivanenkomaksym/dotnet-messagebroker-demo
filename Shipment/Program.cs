using Common.Extensions;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using Shipment;
using Shipment.Consumers;
using Shipment.Data;
using Shipment.Repositories;

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
                .AddMongoDb(hostContext.Configuration.GetConnectionString(), "MongoDb Health", HealthStatus.Degraded);

        BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
    }).Build();

host.Run();
