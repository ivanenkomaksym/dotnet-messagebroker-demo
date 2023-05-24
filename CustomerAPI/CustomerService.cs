using System.Text.Json;
using Common.Events;
using Common.Models;
using MassTransit;

namespace CustomerAPI
{
    public class CustomerService : ICustomerService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(IPublishEndpoint publishEndpoint, ILogger<CustomerService> logger)
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
