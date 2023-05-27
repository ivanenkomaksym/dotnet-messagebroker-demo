using MassTransit;
using OrderProcessor;
using OrderProcessor.Consumers;
using OrderProcessor.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<OrderProcessorWorker>();

        services.AddHttpClient<IOrderService, OrderService>(options =>
        {
            options.BaseAddress = new Uri(hostContext.Configuration["ApiSettings:GatewayAddress"]);
        });

        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderCreatedConsumer>();
            x.AddConsumer<OrderUpdatedConsumer>();
            x.AddConsumer<StockReservedConsumer>();
            x.AddConsumer<ReserveRemovedConsumer>();
            x.AddConsumer<PaymentResultConsumer>();

            x.UsingRabbitMq((context, cfg) => { cfg.ConfigureEndpoints(context); });
        });
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        config.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false);
        config.AddJsonFile("appsettings.k8s.json", optional: true, reloadOnChange: false);
    })
    .Build();

host.Run();
