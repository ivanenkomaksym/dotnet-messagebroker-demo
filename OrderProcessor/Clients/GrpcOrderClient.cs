using Common.Models;
using Common.Models.Payment;
using Common.Models.Shipment;
using Common.Protos;
using Grpc.Net.Client;

namespace OrderProcessor.Clients
{
    internal class GrpcOrderClient : IGrpcOrderClient
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GrpcOrderClient> _logger;

        public GrpcOrderClient(IConfiguration configuration, ILogger<GrpcOrderClient> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Order> GetOrder(Guid orderId)
        {
            _logger.LogInformation($"[GRPC] Sending `GetOrderRequest` event with content: {orderId}");

            using var channel = GrpcChannel.ForAddress(_configuration["GrpcSettings:OrderGrpcUrl"]);
            var client = new OrderService.OrderServiceClient(channel);

            var request = new GetOrderRequest();
            request.OrderId = orderId.ToString();

            var reply = await client.GetOrderAsync(request);

            Guid.TryParse(reply.CustomerInfo.CustomerId, out var customerId);

            return new Order
            {
                Id = orderId,
                OrderStatus = (OrderStatus)reply.OrderStatus,
                PaymentStatus = (PaymentStatus)reply.PaymentStatus,
                DeliveryStatus = (DeliveryStatus)reply.DeliveryStatus,
                CustomerInfo = new Common.Models.CustomerInfo
                {
                    CustomerId = customerId,
                    FirstName = reply.CustomerInfo.FirstName,
                    LastName = reply.CustomerInfo.LastName,
                    Email = reply.CustomerInfo.Email
                },
                PaymentInfo = new Common.Models.Payment.PaymentInfo
                {
                    CardName = reply.PaymentInfo.CardName,
                    CardNumber = reply.PaymentInfo.CardNumber,
                    CVV = reply.PaymentInfo.Cvv,
                    Expiration = reply.PaymentInfo.Expiration,
                    PaymentMethod = (PaymentMethod)reply.PaymentInfo.PaymentMethod
                },
                ShippingAddress = new Common.Models.Address
                {
                    FirstName = reply.ShippingAddress.FirstName,
                    LastName = reply.ShippingAddress.LastName,
                    AddressLine = reply.ShippingAddress.AddressLine,
                    Country = reply.ShippingAddress.Country,
                    Email = reply.ShippingAddress.Email,
                    ZipCode = reply.ShippingAddress.ZipCode
                }
            };
        }

        public async Task<bool> UpdateOrder(Guid orderId,
                                            OrderStatus? orderStatus = null,
                                            PaymentStatus? paymentStatus = null,
                                            DeliveryStatus? deliveryStatus = null)
        {
            _logger.LogInformation($"[GRPC] Sending `UpdateOrderRequest` event with content: {orderId}");

            using var channel = GrpcChannel.ForAddress(_configuration["GrpcSettings:OrderGrpcUrl"]);
            var client = new OrderService.OrderServiceClient(channel);

            var request = new UpdateOrderRequest();
            request.OrderId = orderId.ToString();
            if (orderStatus.HasValue)
                request.OrderStatus = (int)orderStatus.Value;
            if (paymentStatus.HasValue)
                request.PaymentStatus = (int)paymentStatus.Value;
            if (deliveryStatus.HasValue)
                request.DeliveryStatus = (int)deliveryStatus.Value;

            var reply = await client.UpdateOrderAsync(request);

            return reply.Success;
        }
    }
}
