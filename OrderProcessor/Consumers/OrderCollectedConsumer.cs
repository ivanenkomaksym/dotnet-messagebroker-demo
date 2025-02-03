using System.Text.Json;
using Common.Events;
using MassTransit;
using OrderProcessor.Clients;
using OrderProcessor.Discount;

namespace OrderProcessor.Consumers
{
    internal class OrderCollectedConsumer : BaseConsumer<OrderCollected>, IConsumer<OrderCollected>
    {
        private readonly IGrpcOrderClient _grpcOrderClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderCollectedConsumer> _logger;

        public OrderCollectedConsumer(IGrpcOrderClient grpcOrderClient, IPublishEndpoint publishEndpoint, ILogger<OrderCollectedConsumer> logger)
        {
            _grpcOrderClient = grpcOrderClient;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCollected> context)
        {
            // In
            var orderCollected = context.Message;

            await HandleMessage(orderCollected);
        }

        public override async Task HandleMessage(OrderCollected orderCollected)
        {
            var message = JsonSerializer.Serialize(orderCollected);
            _logger.LogInformation($"Received `OrderCollected` event with content: {message}");

            // Out
            await _grpcOrderClient.UpdateOrder(orderCollected.OrderId, orderStatus: Common.Models.OrderStatus.Completed);

            // AddUserCashback
            var order = await _grpcOrderClient.GetOrder(orderCollected.OrderId);
            var cashback = DiscountMessages.GetCashbackPercentageForUser(order.TotalPrice) * order.TotalPrice;
            var promoValidUntil = DateTime.Now + TimeSpan.FromDays(30);

            await _publishEndpoint.SendAddUserCashback(_logger, order, cashback, promoValidUntil);
        }
    }
}