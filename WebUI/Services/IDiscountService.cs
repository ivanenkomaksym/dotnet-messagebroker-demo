using WebUI.Models.Discounts;

namespace WebUI.Services
{
    public interface IDiscountService
    {
        Task<UserPromo?> GetUserPromoForCustomerById(Guid customerId);
    }
}
