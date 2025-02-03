using System.Text.Json;
using Common.Events;
using MassTransit;
using OrderProcessor.Clients;

namespace OrderProcessor.Consumers
{
    internal sealed class ReserveStockResultConsumer : IConsumer<ReserveStockResult>
    {
        private readonly IGrpcOrderClient _grpcOrderClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public ReserveStockResultConsumer(IGrpcOrderClient grpcOrderClient, IPublishEndpoint publishEndpoint, ILogger<OrderCreatedConsumer> logger)
        {
            _grpcOrderClient = grpcOrderClient;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReserveStockResult> context)
        {
            // In
            var reserveStockResult = context.Message;
            var message = JsonSerializer.Serialize(reserveStockResult);
            _logger.LogInformation($"Received `ReserveStockResult` event with content: {message}");

            // Out
            if (reserveStockResult.ReserveResult == Common.Models.Warehouse.ReserveResult.Failed)
            {
                // Stock reserve failed, update order status
                await _grpcOrderClient.UpdateOrder(reserveStockResult.OrderId, orderStatus: Common.Models.OrderStatus.StockReserveFailed);
                return;
            }

            var order = await _grpcOrderClient.GetOrder(reserveStockResult.OrderId);

            var takePaymentEvent = new TakePayment
            {
                OrderId = order.Id,
                CustomerInfo = order.CustomerInfo,
                PaymentInfo = order.PaymentInfo,
                ToBePaidAmount = order.TotalPrice.ToString()
            };

            await _publishEndpoint.Publish(takePaymentEvent);

            // Payment process begins, update order status
            var result = await _grpcOrderClient.UpdateOrder(order.Id, orderStatus: Common.Models.OrderStatus.PaymentProcessing);

            message = JsonSerializer.Serialize(takePaymentEvent);
            _logger.LogInformation($"Sent `TakePayment` event with content: {message}");
        }
    }
}