using Common.Extensions;
using Common.Models;
using Common.Models.Payment;

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

        public async Task<bool> UpdatePayment(Guid orderId, PaymentInfo payment)
        {
            var response = await _client.PutAsJson($"/gateway/Order/{orderId}/Payment", payment);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }

            return await response.ReadContentAs<bool>();
        }

        public async Task<bool> Cancel(Guid orderId)
        {
            var response = await _client.PostAsJson($"/gateway/Order/{orderId}/Cancel", "");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }

            return await response.ReadContentAs<bool>();
        }

        public async Task<bool> Collected(Guid orderId)
        {
            var response = await _client.PostAsJson($"/gateway/Order/{orderId}/Collected", "");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }

            return await response.ReadContentAs<bool>();
        }
    }
}
