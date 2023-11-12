using Common.Extensions;
using Common.Models;
using MongoDB.Driver;

namespace OrderAPI.Data
{
    /// <summary>
    /// This class is at the same time HttpClient in order to get available products from CatalogAPI and seed order data.
    /// </summary>
    internal sealed class OrderContextSeed : IOrderContextSeed
    {
        private readonly HttpClient _client;
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderContextSeed> _logger;

        public OrderContextSeed(HttpClient client, IOrderService orderService, ILogger<OrderContextSeed> logger)
        {
            _client = client;
            _orderService = orderService;
            _logger = logger;
        }

        public async Task SeedData(IMongoCollection<Order> orderCollection)
        {
            var hasAtLeastOneOrder = orderCollection.Find(p => true).Any();
            if (hasAtLeastOneOrder)
                return;

            _logger.LogInformation("SeedData started.");
            var users = await _client.GetUsers();
            var products = await _client.GetProducts();
            _logger.LogInformation($"Received `{products.Count()}` products.");
            var rand = new Random();

            var nofOrders = 10;

            for (var i = 0; i < nofOrders; i++)
            {
                var nofItems = (ushort)rand.Next(1, 3);
                var orderItems = new List<OrderItem>();
                for (var j = 0; j < nofItems; j++)
                {
                    var quantity = (ushort)rand.Next(1, 2);
                    var productIndex = (short)rand.Next(0, products.Count());
                    var product = products.ElementAt(productIndex);

                    orderItems.Add(new OrderItem
                    { 
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        ProductName = product.Name,
                        ProductPrice = product.Price,
                        Quantity = quantity,
                        ImageFile = product.ImageFile
                    });
                }

                var userIndex = (ushort)rand.Next(0, users.Count());
                var user = users.ElementAt(userIndex);

                var paymentMethod = (Common.Models.Payment.PaymentMethod)rand.Next(0, Enum.GetValues<Common.Models.Payment.PaymentMethod>().Length);

                var newOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerInfo = new CustomerInfo
                    {
                        CustomerId = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email
                    },
                    Items = orderItems,
                    ShippingAddress = user.ShippingAddress,
                    PaymentInfo = new Common.Models.Payment.PaymentInfo
                    {
                        CardName = user.PaymentInfo.CardName,
                        CardNumber = user.PaymentInfo.CardNumber,
                        Expiration = user.PaymentInfo.Expiration,
                        CVV = user.PaymentInfo.CVV,
                        PaymentMethod = paymentMethod
                    },
                    UseCashback = 0M,
                    CreationDateTime = DateTime.Now
                };

                await orderCollection.InsertOneAsync(newOrder);

                await _orderService.CreateOrder(newOrder);
            }
            _logger.LogInformation("SeedData ended.");
        }
    }
}
