using System.Text.Json;
using Common.Events;
using MassTransit;

namespace Notifications.Consumers
{
    internal class CustomerCreatedConsumer : IConsumer<CustomerCreated>
    {
        private readonly ILogger<CustomerCreatedConsumer> _logger;

        public CustomerCreatedConsumer(ILogger<CustomerCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<CustomerCreated> context)
        {
            var message = JsonSerializer.Serialize(context.Message);
            _logger.LogInformation($"Received `CustomerCreated` event with content: {message}");

            return Task.CompletedTask;
        }
    }
}
