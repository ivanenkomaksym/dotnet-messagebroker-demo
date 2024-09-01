namespace WebUI.Routing
{
    public interface IEnvironmentRouter
    {
        public string GetCustomerRoute();

        public string GetProductRoute();

        public string GetShoppingCartRoute();

        public string GetDiscountRoute();
    }
}
