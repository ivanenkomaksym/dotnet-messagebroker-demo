using Common.Extensions;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using PaymentService;
using PaymentService.Consumers;
using PaymentService.Data;
using PaymentService.Repositories;

var hostBuilder = Host.CreateDefaultBuilder(args);
hostBuilder.AddServiceDefaults();
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
                .AddMongoDb(hostContext.Configuration.GetConnectionString(), "MongoDb Health", HealthStatus.Degraded);

        BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
    }).Build();

host.Run();
