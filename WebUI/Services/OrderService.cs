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
            var response = await _client.GetAsync($"/gateway/Order/{customerId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
            return await response.ReadContentAs<IEnumerable<Order>>();
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
    }
}
