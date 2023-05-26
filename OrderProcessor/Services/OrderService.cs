using Common.Models;
using Common.Extensions;
using System.Net.Http.Json;

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

        public async Task<bool> UpdateOrder(Order order)
        {
            var response = await _client.PutAsJsonAsync($"/gateway/Order", order);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }

            return await response.ReadContentAs<bool>();
        }
    }
}
