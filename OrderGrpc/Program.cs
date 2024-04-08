using Common.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OrderCommon.Data;
using OrderCommon.Repositories;
using OrderGrpc;

var hostBuilder = Host.CreateDefaultBuilder(args);
hostBuilder.ConfigureOpenTelemetry();

IHost host = hostBuilder
    .ConfigureWebHostDefaults(builder =>
    {
        builder.UseStartup<GrpcServerStartup>();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();

        services.AddScoped<IOrderContext, OrderContextBase>();
        services.AddScoped<IOrderRepository, OrderRepositoryBase>();

        services.AddGrpc();


        services.AddHealthChecks()
                .AddMongoDb(hostContext.Configuration.GetConnectionString(), "MongoDb Health", HealthStatus.Degraded);
    })
    .Build();

host.Run();
