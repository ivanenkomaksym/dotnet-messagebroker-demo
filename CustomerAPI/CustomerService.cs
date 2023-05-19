using System.Text;
using System.Text.Json;
using Common;
using Common.Configuration;
using Common.Models;
using Common.Persistence;
using RabbitMQ.Client;

namespace CustomerAPI
{
    public class CustomerService : ICustomerService
    {
        public CustomerService(IRabbitMQChannelRegistry rabbitMQChannelRegistry, IConfiguration configuration, ILogger<CustomerService> logger)
        {
            RabbitMQChannelRegistry = rabbitMQChannelRegistry;

            var rabbitMQOptions = configuration.GetSection(RabbitMQOptions.Name).Get<RabbitMQOptions>();
            HostName = rabbitMQOptions.HostName;
            Port = rabbitMQOptions.Port;
            Logger = logger;
        }

        public Task CreateCustomer(Customer customer)
        {
            var channel = RabbitMQChannelRegistry.GetOrCreateExchange(HostName, Port, Consts.CustomerStatusExchange, ExchangeType.Fanout, string.Empty, null);

            var message = JsonSerializer.Serialize(customer);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: Consts.CustomerStatusExchange,
                                           routingKey: Consts.CustomerStatusRegistered,
                                 basicProperties: null,
                                 body: body);

            Logger.LogInformation($"['{Consts.CustomerStatusExchange}' exchange] Sent '{message}' with routingKey '{Consts.CustomerStatusRegistered}'");

            return Task.CompletedTask;
        }

        IRabbitMQChannelRegistry RabbitMQChannelRegistry;
        private readonly ILogger<CustomerService> Logger;
        private readonly string HostName;
        private readonly ushort Port;
    }
}
