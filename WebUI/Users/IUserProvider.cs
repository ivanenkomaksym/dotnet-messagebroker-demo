using Common.Models;

namespace WebUI.Users
{
    public interface IUserProvider
    {
        public Guid GetCustomerId(HttpContext context);

        public string GetCustomerEmail(HttpContext httpContext);

        public void SetCustomer(HttpContext httpContext, Customer customer);
    }
}
