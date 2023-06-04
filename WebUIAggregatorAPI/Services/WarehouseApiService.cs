using Common.Models.Warehouse;
using System.Net;

namespace WebUIAggregatorAPI.Services
{
    public class WarehouseApiService : IWarehouseApiService
    {
        private readonly HttpClient _httpClient;

        public WarehouseApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<StockItem> GetStockItemByProductId(Guid productId)
        {
            var response = await _httpClient.GetAsync($"gateway/StockItem/GetStockItemByProductId/{productId}");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            var stock = await response.Content.ReadFromJsonAsync<StockItem>();
            return stock;
        }
    }
}
