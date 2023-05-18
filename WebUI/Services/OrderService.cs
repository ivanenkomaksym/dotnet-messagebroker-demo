using WebUI.Extensions;
using WebUI.Models;

namespace WebUI.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _client;

        public OrderService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<bool> CreateOrder(OrderModel order)
        {
            var response = await _client.PostAsJson($"/gateway/Order", order);
            return await response.ReadContentAs<bool>();
        }
    }
}
