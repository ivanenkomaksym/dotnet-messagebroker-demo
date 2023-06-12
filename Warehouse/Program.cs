using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using Warehouse;
using Warehouse.Consumers;
using WarehouseCommon.Data;
using WarehouseCommon.Repositories;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<WarehouseWorker>();

        services.AddScoped<IWarehouseContext, WarehouseContextBase>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();

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
