using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Common.Models;
using WebUI.Services;
using WebUI.Users;
using MongoDB.Bson.Serialization.IdGenerators;

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
        public Customer Customer { get; set; } = default!;

        [BindProperty]
        public string ErrorMessage { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            try
            { 
                var customer = await _customerService.Authenticate(Customer.Email, Customer.Password);

                _userProvider.SetCustomer(HttpContext, customer);

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
