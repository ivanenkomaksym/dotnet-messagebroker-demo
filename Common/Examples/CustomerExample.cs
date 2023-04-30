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
                Name = "Sam",
                Email = "Sam.Andrew@gmail.com"
            };
        }
    }
}
