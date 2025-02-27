﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Users;

namespace WebUI.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IUserProvider _userProvider;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IUserProvider userProvider, ILogger<LoginModel> logger)
        {
            _userProvider = userProvider;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public required string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public required string Password { get; set; }
        }

        [BindProperty]
        public required InputModel Input { get; set; }

        public string? ReturnUrl { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
                return Page();

            try
            {
                var user = await _userProvider.AuthenticateUserAsync(Input.Email, Input.Password);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }

                await _userProvider.SignInAsync(HttpContext, user);

                _logger.LogInformation("User {Email} logged in at {Time}.",
                    user.Email, DateTime.UtcNow);

                return LocalRedirect(Helpers.GetLocalUrl(Url, returnUrl));
            }
            catch (Exception)
            {
                ModelState.AddModelError(nameof(Input.Email), "Login Failed: Invalid Email or Password");
            }

            return Page();
        }
    }
}