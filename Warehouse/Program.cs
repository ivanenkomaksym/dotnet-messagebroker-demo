using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using Warehouse;
using Warehouse.Consumers;
using Warehouse.Data;
using Warehouse.Repositories;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<WarehouseWorker>();

        services.AddHttpClient<IWarehouseContextSeed, WarehouseContextSeed>(options =>
        {
            options.BaseAddress = new Uri(hostContext.Configuration["ApiSettings:GatewayAddress"]);
        });

        services.AddScoped<IWarehouseContext, WarehouseContext>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ReserveStockConsumer>();

            x.UsingRabbitMq((context, cfg) => { cfg.ConfigureEndpoints(context); });
        });

        services.AddHealthChecks()
                .AddMongoDb(hostContext.Configuration["DatabaseSettings:ConnectionString"], "MongoDb Health", HealthStatus.Degraded);

        BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        config.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false);
        config.AddJsonFile("appsettings.k8s.json", optional: true, reloadOnChange: false);
    })
    .Build();

host.Run();
