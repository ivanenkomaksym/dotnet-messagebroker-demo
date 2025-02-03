using Common.Extensions;
using Common.Models;
using Common.Models.Payment;
using Common.Routing;

namespace WebUI.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _client;
        private readonly string _environmentRoutePrefix;

        public OrderService(HttpClient client, IEnvironmentRouter environmentRouter)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _environmentRoutePrefix = environmentRouter.GetOrderRoute();
        }

        public async Task<IEnumerable<Order>?> GetAllOrders()
        {
            var response = await _client.GetAsync($"{_environmentRoutePrefix}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
            return await response.ReadContentAs<IEnumerable<Order>>();
        }

        public async Task<IEnumerable<Order>?> GetOrders(Guid customerId)
        {
            var response = await _client.GetAsync($"{_environmentRoutePrefix}/GetOrdersByCustomerId/{customerId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
            return await response.ReadContentAs<IEnumerable<Order>>();
        }

        public async Task<Order?> GetOrder(Guid orderId)
        {
            var response = await _client.GetAsync($"{_environmentRoutePrefix}/{orderId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
            return await response.ReadContentAs<Order>();
        }

        public async Task CreateOrder(Order order)
        {
            var response = await _client.PostAsJson($"{_environmentRoutePrefix}", order);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
            return;
        }

        public async Task<bool> UpdatePayment(Guid orderId, PaymentInfo payment)
        {
            var response = await _client.PutAsJson($"{_environmentRoutePrefix}/{orderId}/Payment", payment);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }

            return await response.ReadContentAs<bool>();
        }

        public async Task<bool> Cancel(Guid orderId)
        {
            var response = await _client.PostAsJson($"{_environmentRoutePrefix}/{orderId}/Cancel", "");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }

            return await response.ReadContentAs<bool>();
        }

        public async Task<bool> Collected(Guid orderId)
        {
            var response = await _client.PostAsJson($"{_environmentRoutePrefix}/{orderId}/Collected", "");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }

            return await response.ReadContentAs<bool>();
        }

        public async Task<bool> Return(Guid orderId)
        {
            var response = await _client.PostAsJson($"{_environmentRoutePrefix}/{orderId}/Return", "");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }

            return await response.ReadContentAs<bool>();
        }
    }
}