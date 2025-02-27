﻿using Common.Configuration;
using Microsoft.Extensions.Options;

namespace Common.Routing
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

        public string GetOrderRoute()
        {
            if (_applicationOptions.StartupEnvironment == StartupEnvironment.Aspire)
                return "http://orderapi/api/Order";

            return "/gateway/Order";
        }

        public string GetCatalogRoute()
        {
            if (_applicationOptions.StartupEnvironment == StartupEnvironment.Aspire)
                return "http://catalogapi/api/Catalog";

            return "/gateway/Products";
        }

        public string GetWarehouseRoute()
        {
            if (_applicationOptions.StartupEnvironment == StartupEnvironment.Aspire)
                return "http://warehouseapi/api/StockItem";

            return "/gateway/StockItem";
        }

        public string GetProductRoute()
        {
            if (_applicationOptions.StartupEnvironment == StartupEnvironment.Aspire)
                return "http://webuiaggregatorapi/api/Products";

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