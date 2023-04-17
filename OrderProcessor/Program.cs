using Common.Configuration;
using Common.Persistence;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false);

var configuration = builder.Build();

var apiSettings = configuration.GetSection(RabbitMQOptions.Name).Get<RabbitMQOptions>();

IRabbitMQChannelRegistry rabbitMQChannelRegistry;
if (apiSettings.UseStub)
    rabbitMQChannelRegistry = new StubRabbitMQChannelRegistry();
else
    rabbitMQChannelRegistry = new RabbitMQChannelRegistry();

new OrderProcessor.OrderProcessor(rabbitMQChannelRegistry, apiSettings.HostName);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();