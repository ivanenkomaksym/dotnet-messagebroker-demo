using Common.Examples;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace OrderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            OrderService = orderService;
            Logger = logger;
        }

        [HttpGet]
        public Task<List<Order>> Get()
        {
            return Task.FromResult(new List<Order>());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [SwaggerRequestExample(typeof(OrderExample), typeof(OrderExample))]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            await OrderService.CreateOrder(order);
            return CreatedAtAction(nameof(Get), new { id = order.Id}, order);
        }

        private readonly IOrderService OrderService;
        private readonly ILogger<OrderController> Logger;
    }
}