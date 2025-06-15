using Common.Extensions;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using PaymentService;
using PaymentService.Consumers;
using PaymentService.Data;
using PaymentService.Repositories;

var hostBuilder = Host.CreateDefaultBuilder(args);

IHost host = hostBuilder
    .ConfigureLogging((hostContext, logging) =>
    {
        logging.AddLogging(hostContext.Configuration);
    })
    .ConfigureServices((hostContext, services) =>
    {
        hostBuilder.AddServiceDefaults(hostContext, services);
        services.AddHostedService<PaymentWorker>();

        services.AddScoped<IPaymentContext, PaymentContext>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<TakePaymentConsumer>();
            x.AddConsumer<RefundPaymentConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                Extensions.ConfigureRabbitMq(context, cfg);
                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddHealthChecks()
                        .AddMongoDb(
                            sp => new MongoDB.Driver.MongoClient(hostContext.Configuration.GetConnectionString()),
                            sp => hostContext.Configuration.GetConnectionString(),
                            "MongoDb Health",
                            HealthStatus.Degraded);
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
    }).Build();

host.Run();