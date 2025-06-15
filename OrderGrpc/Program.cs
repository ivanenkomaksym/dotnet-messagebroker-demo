using Common.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
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
                        .AddMongoDb(
                            sp => new MongoDB.Driver.MongoClient(hostContext.Configuration.GetConnectionString()),
                            sp => hostContext.Configuration.GetConnectionString(),
                            "MongoDb Health",
                            HealthStatus.Degraded);
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
    })
    .Build();

host.Run();