using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Services;
using WebUI.Users;

namespace WebUI.Pages.MyAccount
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IUserProvider _userProvider;

        [BindProperty]
        public Customer Customer { get; set; }

        public IndexModel(ICustomerService customerService, IUserProvider userProvider)
        {
            _customerService = customerService;
            _userProvider = userProvider;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var customerId = _userProvider.GetCustomerId(HttpContext);
            Customer = await _customerService.GetCustomerById(customerId);

            return Page();
        }
    }
}
