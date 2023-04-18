using System.Text;
using Common;
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

rabbitMQChannelRegistry.GetOrCreate(apiSettings.HostName, apiSettings.Port, Consts.OrderPaidQueue, (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");
});

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();