using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using OrderCommon.Data;
using OrderCommon.Repositories;
using OrderGrpc;

IHost host = Host.CreateDefaultBuilder(args)
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

        services.AddScoped<IOrderContext, OrderContext>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddGrpc();


        services.AddHealthChecks()
                .AddMongoDb(hostContext.Configuration["DatabaseSettings:ConnectionString"], "MongoDb Health", HealthStatus.Degraded);

        BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
    })
    .Build();

host.Run();
