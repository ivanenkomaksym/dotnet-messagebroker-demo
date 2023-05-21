using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Common.Models;
using WebUI.Services;
using WebUI.Users;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Pages
{
    public class SingInModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IUserProvider _userProvider;

        public SingInModel(ICustomerService customerService, IUserProvider userProvider)
        {
            _customerService = customerService;
            _userProvider = userProvider;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [BindProperty]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            { 
                var customer = await _customerService.Authenticate(Email, Password);

                _userProvider.SetCustomer(HttpContext, customer);

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(Email), "Login Failed: Invalid Email or Password");
            }

            return Page();
        }
    }
}
