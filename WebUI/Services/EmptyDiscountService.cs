using WebUI.Models.Discounts;

namespace WebUI.Services
{
    public class EmptyDiscountService : IDiscountService
    {
        public Task<UserPromo> GetUserPromoForCustomerById(Guid customerId) => Task.FromResult(new UserPromo());
    }
}
