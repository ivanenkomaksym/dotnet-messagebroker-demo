using Shipment;
using Shipment.Data;
using Shipment.Repositories;
using Shipment.Consumers;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();

        services.AddScoped<IShipmentContext, ShipmentContext>();
        services.AddScoped<IShipmentRepository, ShipmentRepository>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ShipOrderConsumer>();

            x.UsingRabbitMq((context, cfg) => { cfg.ConfigureEndpoints(context); });
        });

        services.AddHealthChecks()
                .AddMongoDb(hostContext.Configuration["DatabaseSettings:ConnectionString"], "MongoDb Health", HealthStatus.Degraded);

        BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
    }).Build();

host.Run();
