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
        public required IEnumerable<Customer> Users { get; set; }

        public UsersModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task OnGetAsync()
        {
            // Get existing users
            var users = await _customerService.GetCustomers();
            ArgumentNullException.ThrowIfNull(users);
            Users = users;
        }
    }
}