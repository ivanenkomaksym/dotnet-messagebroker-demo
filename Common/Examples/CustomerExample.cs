using Common.Models;
using Common.SeedData;
using Swashbuckle.AspNetCore.Filters;

namespace Common.Examples
{
    public class CustomerExample : IExamplesProvider<Customer>
    {
        public Customer GetExamples()
        {
            return CustomerSeed.GetPreconfiguredCustomer().First();
        }
    }
}