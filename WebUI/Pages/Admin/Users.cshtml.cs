using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Services;

namespace WebUI.Pages.Admin
{
    public class UsersModel : PageModel
    {
        private readonly ICustomerService _customerService;

        [BindProperty]
        public IEnumerable<Customer> Users { get; private set; }

        public UsersModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task OnGetAsync()
        {
            // Get existing users
            Users = await _customerService.GetCustomers();
        }
    }
}
