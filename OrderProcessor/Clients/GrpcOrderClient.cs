using Common.Models;
using Common.Models.Payment;
using Common.Protos;

namespace OrderProcessor.Clients
{
    internal class GrpcOrderClient : IGrpcOrderClient
    {
        private readonly ILogger<GrpcOrderClient> _logger;
        private readonly OrderService.OrderServiceClient _client;

        public GrpcOrderClient(OrderService.OrderServiceClient client, ILogger<GrpcOrderClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<Order> GetOrder(Guid orderId)
        {
            _logger.LogInformation($"[GRPC] Sending `GetOrderRequest` event with content: {orderId}");

            var request = new GetOrderRequest();
            request.OrderId = orderId.ToString();

            var reply = await _client.GetOrderAsync(request);

            Guid.TryParse(reply.CustomerInfo.CustomerId, out var customerId);

            var items = new List<Common.Models.OrderItem>();
            foreach (var item in reply.Items)
            {
                Guid.TryParse(item.Id, out var itemId);
                Guid.TryParse(item.ProductId, out var productId);
                items.Add(new Common.Models.OrderItem
                {
                    Id = itemId,
                    ProductId = productId,
                    ProductName = item.ProductName,
                    ProductPrice = decimal.Parse(item.ProductPrice),
                    Quantity = (ushort)item.Quantity,
                    ImageFile = item.ImageFile
                });
            }

            return new Order
            {
                Id = orderId,
                OrderStatus = (OrderStatus)reply.OrderStatus,
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
                },
                Items = items,
                TotalPrice = decimal.Parse(reply.TotalPrice)
            };
        }

        public async Task<bool> UpdateOrder(Guid orderId, OrderStatus orderStatus)
        {
            _logger.LogInformation($"[GRPC] Sending `UpdateOrderRequest` event with content: {orderId}");

            var request = new UpdateOrderRequest();
            request.OrderId = orderId.ToString();
            request.OrderStatus = (int)orderStatus;

            var reply = await _client.UpdateOrderAsync(request);

            return reply.Success;
        }
    }
}
