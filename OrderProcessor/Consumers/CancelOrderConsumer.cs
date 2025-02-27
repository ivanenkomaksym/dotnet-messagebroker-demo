﻿using System.Text.Json;
using Common.Events;
using MassTransit;
using OrderProcessor.Clients;

namespace OrderProcessor.Consumers
{
    internal class CancelOrderConsumer : BaseConsumer<CancelOrder>, IConsumer<CancelOrder>
    {
        private readonly IGrpcOrderClient _grpcOrderClient;
        private readonly ILogger<CancelOrderConsumer> _logger;

        public CancelOrderConsumer(IGrpcOrderClient grpcOrderClient, ILogger<CancelOrderConsumer> logger)
        {
            _grpcOrderClient = grpcOrderClient;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CancelOrder> context)
        {
            // In
            var cancelOrder = context.Message;

            await HandleMessage(cancelOrder);
        }

        public override async Task HandleMessage(CancelOrder cancelOrder)
        {
            var message = JsonSerializer.Serialize(cancelOrder);
            _logger.LogInformation($"Received `CancelOrder` event with content: {message}");

            // Out
            var result = await _grpcOrderClient.UpdateOrder(cancelOrder.OrderId, orderStatus: Common.Models.OrderStatus.Cancelled);
        }
    }
}