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

        [BindProperty]
        public string ErrorMessage { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _customerService.CreateCustomer(Customer);

                _userProvider.SetCustomer(HttpContext, Customer);

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return Page();
        }
    }
}
