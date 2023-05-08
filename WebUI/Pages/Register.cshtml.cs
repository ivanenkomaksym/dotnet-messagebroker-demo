using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Common.Models;
using WebUI.Services;

namespace WebUI.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public RegisterModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Customer Customer { get; set; } = default!;

        [TempData]
        public string Username { get; set; }
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            await _customerService.CreateCustomer(Customer);

            ViewData["username"] = Customer.Name;
            Username = Customer.Name;

            return RedirectToPage("./Index");
        }
    }
}
