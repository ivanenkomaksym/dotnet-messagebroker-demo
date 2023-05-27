using Common.Extensions;
using Common.Models;

namespace WebUI.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _client;

        public OrderService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<Order>> GetOrders(Guid customerId)
        {
            var response = await _client.GetAsync($"/gateway/Order/GetOrdersByCustomerId/{customerId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
            return await response.ReadContentAs<IEnumerable<Order>>();
        }

        public async Task<Order> GetOrder(Guid orderId)
        {
            var response = await _client.GetAsync($"/gateway/Order/{orderId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
            return await response.ReadContentAs<Order>();
        }

        public async Task CreateOrder(Order order)
        {
            var response = await _client.PostAsJson($"/gateway/Order", order);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
            return;
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
