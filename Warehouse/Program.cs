using Warehouse;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WarehouseWorker>();
    })
    .Build();

host.Run();
