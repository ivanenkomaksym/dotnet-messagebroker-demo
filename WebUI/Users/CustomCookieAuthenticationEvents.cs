using Microsoft.AspNetCore.Authentication.Cookies;
using WebUI.Consumers;

namespace WebUI.Users
{
    /// <summary>
    /// This event handler will make sure to lazy register customer id in IMassTransitConsumersRegistry whenever cookie authentication is challenged.
    /// </summary>
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly IMassTransitConsumersRegistry _massTransitConsumersRegistry;

        public CustomCookieAuthenticationEvents(IMassTransitConsumersRegistry massTransitConsumersRegistry)
        {
            _massTransitConsumersRegistry = massTransitConsumersRegistry;
        }

        public override Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;

            // Look for the CustomerId claim.
            var customerIdString = (from c in userPrincipal?.Claims
                                    where c.Type == "CustomerId"
                                    select c.Value).FirstOrDefault();

            if (!string.IsNullOrEmpty(customerIdString))
            {
                Guid.TryParse(customerIdString, out var customerId);
                _massTransitConsumersRegistry.StartListeningForApplicationUser(customerId);
            }

            return base.ValidatePrincipal(context);
        }
    }
}