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
    }).Build();

host.Run();
