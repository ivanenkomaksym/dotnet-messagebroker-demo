using Common.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using OrderCommon.Data;
using OrderCommon.Repositories;
using OrderGrpc;

var hostBuilder = Host.CreateDefaultBuilder(args);

IHost host = hostBuilder
    .ConfigureLogging((hostContext, logging) =>
    {
        logging.AddLogging(hostContext.Configuration);
    })
    .ConfigureWebHostDefaults(builder =>
    {
        builder.UseStartup<GrpcServerStartup>();
    })
    .ConfigureServices((hostContext, services) =>
    {
        hostBuilder.AddServiceDefaults(hostContext, services);
        services.AddHostedService<Worker>();

        services.AddScoped<IOrderContext, OrderContextBase>();
        services.AddScoped<IOrderRepository, OrderRepositoryBase>();

        services.AddGrpc();


        services.AddHealthChecks()
                .AddMongoDb(hostContext.Configuration.GetConnectionString(), "MongoDb Health", HealthStatus.Degraded);

        BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
    })
    .Build();

host.Run();
