using Common.Extensions;
using WebUI.Models.Discounts;
using WebUI.Routing;

namespace WebUI.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly HttpClient _client;
        private readonly string _environmentRoutePrefix;

        public DiscountService(HttpClient client, IEnvironmentRouter environmentRouter)
        {
            _client = client;
            _environmentRoutePrefix = environmentRouter.GetCustomerRoute();
        }

        public async Task<UserPromo?> GetUserPromoForCustomerById(Guid customerId)
        {
            var response = await _client.GetAsync($"{_environmentRoutePrefix}/{customerId}");
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<UserPromo>();
            else
                return null;
        }
    }
}
