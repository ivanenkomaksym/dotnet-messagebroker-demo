using Common.Models;
using WebUI.Extensions;

namespace WebUI.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _client;

        public OrderService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<bool> CreateOrder(Order order)
        {
            var response = await _client.PostAsJson($"/gateway/Order", order);
            return await response.ReadContentAs<bool>();
        }
    }
}
