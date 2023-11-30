using Common.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using OrderCommon.Data;
using OrderCommon.Repositories;
using OrderGrpc;

var hostBuilder = Host.CreateDefaultBuilder(args);
hostBuilder.ConfigureOpenTelemetry();

IHost host = hostBuilder
    .ConfigureWebHostDefaults(builder =>
    {
        builder
            //.ConfigureKestrel(options =>
            //{
            //    options.ListenAnyIP(0, listenOptions =>
            //    {
            //        listenOptions.Protocols = HttpProtocols.Http2;
            //    });
            //})
            //.UseKestrel()
            .UseStartup<GrpcServerStartup>();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();

        services.AddScoped<IOrderContext, OrderContextBase>();
        services.AddScoped<IOrderRepository, OrderRepositoryBase>();

        services.AddGrpc();


        services.AddHealthChecks()
                .AddMongoDb(hostContext.Configuration["DatabaseSettings:ConnectionString"], "MongoDb Health", HealthStatus.Degraded);

        BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
    })
    .Build();

host.Run();
