﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Common.Models;
using WebUI.Services;
using WebUI.Users;
using WebUI.Data;

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

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                await _customerService.CreateCustomer(Customer);

                var user = new ApplicationUser
                {
                    CustomerId = Customer.Id,
                    Email = Customer.Email,
                    FullName = $"{Customer.FirstName} {Customer.LastName}"
                };

                await _userProvider.SignInAsync(HttpContext, user);

                return RedirectToPage("./Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError(nameof(Customer.Email), "User with this email already exists.");
            }

            return Page();
        }
    }
}
