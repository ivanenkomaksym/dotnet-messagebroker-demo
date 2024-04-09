using System.Text.Json;
using Common.Events;
using Common.Models;
using MassTransit;

namespace CustomerAPI.Messaging
{
    internal class CustomerPublisher : ICustomerPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CustomerPublisher> _logger;

        public CustomerPublisher(IPublishEndpoint publishEndpoint, ILogger<CustomerPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task CreateCustomer(Customer customer)
        {
            var customerCreatedEvent = new CustomerCreated
            {
                CustomerInfo = new CustomerInfo
                {
                    CustomerId = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email
                },
                CreationDateTime = DateTime.Now
            };

            await _publishEndpoint.Publish(customerCreatedEvent);

            var message = JsonSerializer.Serialize(customerCreatedEvent);
            _logger.LogInformation($"Sent `CustomerCreated` event with content: {message}");
        }
    }
}
