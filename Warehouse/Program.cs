using Common.Extensions;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using Warehouse;
using Warehouse.Consumers;
using WarehouseCommon.Data;
using WarehouseCommon.Repositories;

var hostBuilder = Host.CreateDefaultBuilder(args);

IHost host = hostBuilder
    .ConfigureLogging((hostContext, logging) =>
    {
        logging.AddLogging(hostContext.Configuration);
    })
    .ConfigureServices((hostContext, services) =>
    {
        hostBuilder.AddServiceDefaults(hostContext, services);
        services.AddHostedService<WarehouseWorker>();

        services.AddSingleton<IWarehouseContextSeed, WarehouseContextSeed>();

        services.AddSingleton<IWarehouseContext, WarehouseContext>();
        services.AddSingleton<IWarehouseRepository, WarehouseRepository>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ReserveStockConsumer>();
            x.AddConsumer<RemoveReserveConsumer>();
            x.AddConsumer<UpdateStockConsumer>();

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