using Common.Models;
using Common.Protos;
using OrderCommon.Repositories;
using Grpc.Core;
using System.Diagnostics;

namespace OrderAPI.Services
{
    internal class GrpcOrderService : OrderService.OrderServiceBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GrpcOrderService> _logger;

        public GrpcOrderService(IOrderRepository orderRepository, ILogger<GrpcOrderService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public override async Task<GetOrderReply> GetOrder(GetOrderRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"[GRPC] Receiving `GetOrderRequest` event with orderId: {request.OrderId}");

            Guid.TryParse(request.OrderId, out var orderId);
            var order = await _orderRepository.GetOrderById(orderId);
            // TODO: add error handling

            return new GetOrderReply
            {
                OrderId = order.Id.ToString(),
                OrderStatus = (int)order.OrderStatus,
                CustomerInfo = new Common.Protos.CustomerInfo()
                {
                    CustomerId = order.CustomerInfo.CustomerId.ToString(),
                    FirstName = order.CustomerInfo.FirstName,
                    LastName = order.CustomerInfo.LastName,
                    Email = order.CustomerInfo.Email
                },
                ShippingAddress = new Common.Protos.Address()
                {
                    FirstName = order.ShippingAddress.FirstName,
                    LastName = order.ShippingAddress.LastName,
                    Country = order.ShippingAddress.Country,
                    AddressLine = order.ShippingAddress.AddressLine,
                    Email = order.ShippingAddress.Email,
                    ZipCode = order.ShippingAddress.ZipCode
                },
                PaymentInfo = new Common.Protos.PaymentInfo()
                {
                    CardName = order.PaymentInfo.CardName,
                    CardNumber = order.PaymentInfo.CardNumber,
                    Cvv = order.PaymentInfo.CVV,
                    Expiration = order.PaymentInfo.Expiration,
                    PaymentMethod = (int)order.PaymentInfo.PaymentMethod
                }
            };
        }

        public override async Task<UpdateOrderReply> UpdateOrder(UpdateOrderRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"[GRPC] Receiving `UpdateOrderRequest` event with orderId: {request.OrderId}");

            Guid.TryParse(request.OrderId, out var orderId);
            var order = await _orderRepository.GetOrderById(orderId);
            if (order == null) return new UpdateOrderReply { Success = false };

            order.OrderStatus = (OrderStatus)request.OrderStatus;

            var result = await _orderRepository.UpdateOrder(order);
            if (!result) return new UpdateOrderReply { Success = false };

            return new UpdateOrderReply { Success = true };
        }
    }
}
