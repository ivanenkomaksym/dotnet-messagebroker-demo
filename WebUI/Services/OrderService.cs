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
