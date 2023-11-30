using Common.Extensions;
using PaymentService;
using PaymentService.Consumers;
using PaymentService.Data;
using PaymentService.Repositories;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;

var hostBuilder = Host.CreateDefaultBuilder(args);
hostBuilder.ConfigureOpenTelemetry();

IHost host = hostBuilder
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<PaymentWorker>();

        services.AddScoped<IPaymentContext, PaymentContext>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<TakePaymentConsumer>();
            x.AddConsumer<RefundPaymentConsumer>();

            x.UsingRabbitMq((context, cfg) => { cfg.ConfigureEndpoints(context); });
        });

        services.AddHealthChecks()
                .AddMongoDb(hostContext.Configuration["DatabaseSettings:ConnectionString"], "MongoDb Health", HealthStatus.Degraded);

        BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
    }).Build();

host.Run();
