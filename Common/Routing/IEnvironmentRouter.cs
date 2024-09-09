namespace Common.Routing
{
    public interface IEnvironmentRouter
    {
        public string GetCustomerRoute();

        public string GetCatalogRoute();

        public string GetWarehouseRoute();

        public string GetProductRoute();

        public string GetShoppingCartRoute();

        public string GetOrderRoute();
    }
}
