using Common.Extensions;
using Common.Routing;
using WebUI.Models.Discounts;

namespace WebUI.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly HttpClient _client;

        public DiscountService(HttpClient client, IEnvironmentRouter environmentRouter)
        {
            _client = client;
        }

        public async Task<UserPromo?> GetUserPromoForCustomerById(Guid customerId)
        {
            var response = await _client.GetAsync($"/gateway/userpromos/{customerId}");
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<UserPromo>();
            else
                return null;
        }
    }
}
