using Common.Configuration;
using Microsoft.Extensions.Options;

namespace WebUI.Routing
{
    public class EnvironmentRouter : IEnvironmentRouter
    {
        private readonly ApplicationOptions _applicationOptions;

        public EnvironmentRouter(IOptions<ApplicationOptions> applicationOptions)
        {
            ArgumentNullException.ThrowIfNull(applicationOptions);
            _applicationOptions = applicationOptions.Value;
        }

        public string GetCustomerRoute()
        {
            if (_applicationOptions.StartupEnvironment == StartupEnvironment.Aspire)
                return "http://customerapi/api/Customer";

            return "/gateway/Customer";
        }

        public string GetDiscountRoute()
        {
            if (_applicationOptions.StartupEnvironment == StartupEnvironment.Aspire)
                return "http://discountapi/api";

            return "/gateway/userpromos";
        }

        public string GetProductRoute()
        {
            if (_applicationOptions.StartupEnvironment == StartupEnvironment.Aspire)
                return "http://catalogapi/api/Catalog";

            return "/gateway/Products";
        }

        public string GetShoppingCartRoute()
        {
            if (_applicationOptions.StartupEnvironment == StartupEnvironment.Aspire)
                return "http://shoppingcartapi/api/ShoppingCart";

            return "/gateway/ShoppingCart";
        }
    }
}
