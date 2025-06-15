using Common.Extensions;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using Shipment;
using Shipment.Consumers;
using Shipment.Data;
using Shipment.Repositories;

var hostBuilder = Host.CreateDefaultBuilder(args);

IHost host = hostBuilder
    .ConfigureLogging((hostContext, logging) =>
    {
        logging.AddLogging(hostContext.Configuration);
    })
    .ConfigureServices((hostContext, services) =>
    {
        hostBuilder.AddServiceDefaults(hostContext, services);
        services.AddHostedService<Worker>();

        services.AddScoped<IShipmentContext, ShipmentContext>();
        services.AddScoped<IShipmentRepository, ShipmentRepository>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ShipOrderConsumer>();
            x.AddConsumer<ShipmentToBeReturnedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                Extensions.ConfigureRabbitMq(context, cfg);
                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddHealthChecks()
                        .AddMongoDb(
                            sp => new MongoDB.Driver.MongoClient(hostContext.Configuration.GetConnectionString()),
                            sp => hostContext.Configuration.GetConnectionString(),
                            "MongoDb Health",
                            HealthStatus.Degraded);
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
    }).Build();

host.Run();