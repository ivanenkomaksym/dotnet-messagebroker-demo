using Common.Extensions;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using Warehouse;
using Warehouse.Consumers;
using WarehouseCommon.Data;
using WarehouseCommon.Repositories;

var hostBuilder = Host.CreateDefaultBuilder(args);
hostBuilder.ConfigureOpenTelemetry();

IHost host = hostBuilder
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<WarehouseWorker>();

        services.AddSingleton<IWarehouseContextSeed, WarehouseContextSeed>();

        services.AddSingleton<IWarehouseContext, WarehouseContext>();
        services.AddSingleton<IWarehouseRepository, WarehouseRepository>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ReserveStockConsumer>();
            x.AddConsumer<RemoveReserveConsumer>();
            x.AddConsumer<UpdateStockConsumer>();

            x.UsingRabbitMq((context, cfg) => { cfg.ConfigureEndpoints(context); });
        });

        services.AddHealthChecks()
                .AddMongoDb(hostContext.Configuration["DatabaseSettings:ConnectionString"], "MongoDb Health", HealthStatus.Degraded);

        BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
    }).Build();

host.Run();
