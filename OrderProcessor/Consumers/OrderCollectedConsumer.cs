using System.Text.Json;
using Common.Events;
using OrderProcessor.Clients;
using MassTransit;

namespace OrderProcessor.Consumers
{
    internal class OrderCollectedConsumer : IConsumer<OrderCollected>
    {
        private readonly IGrpcOrderClient _grpcOrderClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderCollectedConsumer> _logger;

        public OrderCollectedConsumer(IGrpcOrderClient grpcOrderClient, IPublishEndpoint publishEndpoint , ILogger<OrderCollectedConsumer> logger)
        {
            _grpcOrderClient = grpcOrderClient;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCollected> context)
        {
            // In
            var orderCollected = context.Message;
            var message = JsonSerializer.Serialize(orderCollected);
            _logger.LogInformation($"Received `OrderCollected` event with content: {message}");

            // Out
            await _grpcOrderClient.UpdateOrder(orderCollected.OrderId, orderStatus: Common.Models.OrderStatus.Completed);

            // AddUserPromo
            var order = await _grpcOrderClient.GetOrder(orderCollected.OrderId);
            var promoValidUntil = DateTime.Now + TimeSpan.FromDays(30);

            var addUserPromo = new AddUserPromo
            {
                CustomerInfo = order.CustomerInfo,
                Promo = GetPromoForUser(order.TotalPrice),
                ValidUntil = promoValidUntil
            };

            await _publishEndpoint.Publish(addUserPromo);

            message = JsonSerializer.Serialize(addUserPromo);
            _logger.LogInformation($"Sent `AddUserPromo` event with content: {message}");
        }

        // To be moved to Discount microservice
        public static double GetPromoForUser(double totalPrice)
        {
            if (totalPrice < 50.0)
                return 0.05;
            if (totalPrice < 100.0)
                return 0.1;

            return 0.15;
        }
    }
}
