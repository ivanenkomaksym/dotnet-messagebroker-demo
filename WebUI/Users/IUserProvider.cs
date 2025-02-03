using WebUI.Data;

namespace WebUI.Users
{
    public interface IUserProvider
    {
        public Guid GetCustomerId(HttpContext context);

        public Task<ApplicationUser?> AuthenticateUserAsync(string email, string password);

        public Task SignInAsync(HttpContext httpContext, ApplicationUser user);

        public Task SignOutAsync(HttpContext httpContext);
    }
}