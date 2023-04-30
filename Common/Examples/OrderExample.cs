using Common.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Common.Examples
{
    public class OrderExample : IExamplesProvider<Order>
    {
        public Order GetExamples()
        {
            return new Order
            {
                Id = Guid.NewGuid(),
                ProductId = "prd01",
                Quantity = 1
            };
        }
    }
}
