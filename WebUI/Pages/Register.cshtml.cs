using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Common.Models;
using WebUI.Services;
using WebUI.Users;

namespace WebUI.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IUserProvider _userProvider;

        public RegisterModel(ICustomerService customerService, IUserProvider userProvider)
        {
            _customerService = customerService;
            _userProvider = userProvider;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Customer Customer { get; set; } = default!;

        [TempData]
        public string Username { get; set; }

        [TempData]
        public Guid CustomerId { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            await _customerService.CreateCustomer(Customer);

            _userProvider.SetCustomer(HttpContext, Customer);
            Username = Customer.Name;

            return RedirectToPage("./Index");
        }
    }
}
