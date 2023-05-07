using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Common.Models;
using WebUI.Services;

namespace WebUI.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public CreateModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Customer Customer { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            await _customerService.CreateCustomer(Customer);

            return RedirectToPage("./Index");
        }
    }
}
