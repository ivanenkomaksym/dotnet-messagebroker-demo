using Common.Models.Warehouse;
using System.Net;
using System.Text.Json;
using System.Text;
using Common.Routing;

namespace WebUIAggregatorAPI.Services
{
    public class WarehouseApiService : IWarehouseApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _environmentRoutePrefix;

        public WarehouseApiService(HttpClient httpClient, IEnvironmentRouter environmentRouter)
        {
            _httpClient = httpClient;
            _environmentRoutePrefix = environmentRouter.GetWarehouseRoute();
        }

        public async Task<StockItem?> GetStockItemByProductId(Guid productId)
        {
            var response = await _httpClient.GetAsync($"{_environmentRoutePrefix}/GetStockItemByProductId/{productId}");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<StockItem>();
        }

        public async Task<StockItem?> CreateStockItem(StockItem stockItem)
        {
            var content = new StringContent(JsonSerializer.Serialize(stockItem), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_environmentRoutePrefix}", content);
            response.EnsureSuccessStatusCode();

            var createdStockItem = await response.Content.ReadFromJsonAsync<StockItem>();
            return createdStockItem;
        }

        public async Task<StockItem?> UpdateStockItem(StockItem stockItem)
        {
            var content = new StringContent(JsonSerializer.Serialize(stockItem), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_environmentRoutePrefix}", content);
            response.EnsureSuccessStatusCode();

            var updatedStockItem = await response.Content.ReadFromJsonAsync<StockItem>();
            return updatedStockItem;
        }

        public async Task<bool> DeleteStockItemByProductId(Guid productId)
        {
            var response = await _httpClient.DeleteAsync($"{_environmentRoutePrefix}/DeleteStockItemByProductId/{productId}");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            response.EnsureSuccessStatusCode();
            return true;
        }
    }
}
