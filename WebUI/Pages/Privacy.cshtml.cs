using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace WebUI.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly IToastNotification _toastNotification;
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(IToastNotification toastNotification, ILogger<PrivacyModel> logger)
        {
            _toastNotification = toastNotification;
            _logger = logger;
        }

        public void OnGet()
        {
            // Success Toast
            _toastNotification.AddSuccessToastMessage("Woo hoo - it works!");

            // Info Toast
            _toastNotification.AddInfoToastMessage("Here is some information.");

            // Error Toast
            _toastNotification.AddErrorToastMessage("Woops an error occured.");

            // Warning Toast
            _toastNotification.AddWarningToastMessage("Here is a simple warning!");
        }
    }
}