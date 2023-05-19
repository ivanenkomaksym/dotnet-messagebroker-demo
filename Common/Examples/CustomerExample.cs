using Common.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Common.Examples
{
    public class CustomerExample : IExamplesProvider<Customer>
    {
        public Customer GetExamples()
        {
            return new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Alice",
                LastName = "Liddell",
                Email = "alice@gmail.com",
                Password = "password"
            };
        }
    }
}
