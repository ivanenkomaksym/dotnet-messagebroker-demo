using Common.Events;
using MassTransit;
using OrderProcessor.Clients;
using System.Text.Json;

namespace OrderProcessor.Consumers
{
    internal sealed class StockReservedConsumer : IConsumer<StockReserved>
    {
        private readonly IGrpcOrderClient _grpcOrderClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public StockReservedConsumer(IGrpcOrderClient grpcOrderClient, IPublishEndpoint publishEndpoint, ILogger<OrderCreatedConsumer> logger)
        {
            _grpcOrderClient = grpcOrderClient;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StockReserved> context)
        {
            // In
            var stockReserved = context.Message;
            var message = JsonSerializer.Serialize(stockReserved);
            _logger.LogInformation($"Received `StockReserved` event with content: {message}");

            // Out
            var order = await _grpcOrderClient.GetOrder(stockReserved.OrderId);

            var takePaymentEvent = new TakePayment
            {
                OrderId = order.Id,
                CustomerInfo = order.CustomerInfo,
                PaymentInfo = order.PaymentInfo,
                ToBePaidAmount = order.TotalPrice
            };

            await _publishEndpoint.Publish(takePaymentEvent);

            // Payment process begins, update order status
            var result = await _grpcOrderClient.UpdateOrder(order.Id, orderStatus: Common.Models.OrderStatus.PaymentProcessing);

            message = JsonSerializer.Serialize(takePaymentEvent);
            _logger.LogInformation($"Sent `TakePayment` event with content: {message}");
        }
    }
}
