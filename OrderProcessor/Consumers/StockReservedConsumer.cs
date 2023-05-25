using Common.Events;
using MassTransit;
using OrderProcessor.Services;
using System.Text.Json;

namespace OrderProcessor.Consumers
{
    internal sealed class StockReservedConsumer : IConsumer<StockReserved>
    {
        private readonly IOrderService _orderService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public StockReservedConsumer(IOrderService orderService, IPublishEndpoint publishEndpoint, ILogger<OrderCreatedConsumer> logger)
        {
            _orderService = orderService;
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
            var order = await _orderService.GetOrderById(stockReserved.OrderId);

            var takePaymentEvent = new TakePayment
            {
                OrderId = order.Id,
                CustomerInfo = order.CustomerInfo,
                PaymentInfo = order.PaymentInfo
            };

            await _publishEndpoint.Publish(takePaymentEvent);

            message = JsonSerializer.Serialize(takePaymentEvent);
            _logger.LogInformation($"Sent `TakePayment` event with content: {message}");
        }
    }
}
