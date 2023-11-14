using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement.Mvc;
using WebUI.Models.Discounts;
using WebUI.Services;
using WebUI.Users;

namespace WebUI.Pages.MyAccount
{
    [FeatureGate(FeatureFlags.Discount)]
    public class DiscountsModel : PageModel
    {
        private readonly IDiscountService _discountService;
        private readonly IUserProvider _userProvider;

        [BindProperty]
        public UserPromo UserPromo { get; set; }

        public DiscountsModel(IDiscountService discountService, IUserProvider userProvider)
        {
            _discountService = discountService;
            _userProvider = userProvider;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);
            UserPromo = await _discountService.GetUserPromoForCustomerById(customerId);

            return Page();
        }
    }
}
