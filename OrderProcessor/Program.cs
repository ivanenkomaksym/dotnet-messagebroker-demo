﻿using OrderProcessor;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<OrderProcessorWorker>();
    })
    .Build();

host.Run();
