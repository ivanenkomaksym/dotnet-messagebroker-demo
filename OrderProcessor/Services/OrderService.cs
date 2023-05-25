using Common.Models;
using Common.Extensions;

namespace OrderProcessor.Services
{
    internal sealed class OrderService : IOrderService
    {
        private readonly HttpClient _client;

        public OrderService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<Order> GetOrderById(Guid orderId)
        {
            var response = await _client.GetAsync($"/gateway/Order/{orderId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
            return await response.ReadContentAs<Order>();
        }
    }
}
