using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Users;

namespace WebUI.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly IUserProvider _userProvider;
        private readonly ILogger<LoginModel> _logger;

        public LogoutModel(IUserProvider userProvider, ILogger<LoginModel> logger)
        {
            _userProvider = userProvider;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("User {Name} logged out at {Time}.", User.Identity.Name, DateTime.UtcNow);

            await _userProvider.SignOutAsync(HttpContext);

            return RedirectToPage("SignedOut");
        }
    }
}
