using Warehouse;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WarehouseWorker>();
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        config.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false);
        config.AddJsonFile("appsettings.k8s.json", optional: true, reloadOnChange: false);
    })
    .Build();

host.Run();
