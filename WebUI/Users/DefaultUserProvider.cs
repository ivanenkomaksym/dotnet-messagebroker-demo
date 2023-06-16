using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using WebUI.Data;
using WebUI.Services;
using WebUI.Consumers;

namespace WebUI.Users
{
    public class DefaultUserProvider : IUserProvider
    {
        private readonly ICustomerService _customerService;
        private readonly IMassTransitConsumersRegistry _massTransitConsumersRegistry;

        public DefaultUserProvider(ICustomerService customerService, IMassTransitConsumersRegistry massTransitConsumersRegistry)
        {
            _customerService = customerService;
            _massTransitConsumersRegistry = massTransitConsumersRegistry;
        }

        public Guid GetCustomerId(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                Guid sessionId = Guid.Empty;

                var hasSessionId = httpContext.Request.Cookies.TryGetValue("sessionId", out var sessionIdStr);
                if (hasSessionId)
                {
                    var result = Guid.TryParse(sessionIdStr, out sessionId);
                    if (result)
                        return sessionId;
                }

                sessionId = Guid.NewGuid();
                httpContext.Response.Cookies.Append("sessionId", sessionId.ToString());

                return sessionId;
            }

            Guid.TryParse(httpContext.User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value!, out var customerId);
            return customerId;
        }

        public async Task<ApplicationUser> AuthenticateUserAsync(string email, string password)
        {
            var customer = await _customerService.Authenticate(email, password);
            if (customer != null)
            {
                return new ApplicationUser()
                {
                    CustomerId = customer.Id,
                    Email = customer.Email,
                    FullName = $"{customer.FirstName} {customer.LastName}"
                };
            }
            else
            {
                return null;
            }
        }

        public async Task SignInAsync(HttpContext httpContext, ApplicationUser user)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("CustomerId", user.CustomerId.ToString()),
                    new Claim("FullName", user.FullName),
                    new Claim(ClaimTypes.Role, "User"),
                };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        public async Task SignOutAsync(HttpContext httpContext)
        {
            var customerId = GetCustomerId(httpContext);
            if (customerId != Guid.Empty)
                await _massTransitConsumersRegistry.StopListeningForApplicationUser(customerId);

            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
