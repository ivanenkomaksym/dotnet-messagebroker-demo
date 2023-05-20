using Common.Examples;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.Repositories;
using Swashbuckle.AspNetCore.Filters;

namespace OrderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        public OrderController(IOrderService orderService, IOrderRepository orderRepository, ILogger<OrderController> logger)
        {
            OrderService = orderService;
            OrderRepository = orderRepository;
            Logger = logger;
        }

        [HttpGet("{customerId}", Name = "GetOrders")]
        public async Task<ActionResult<List<Order>>> Get(Guid customerId)
        {
            return Ok(await OrderRepository.GetOrders(customerId));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [SwaggerRequestExample(typeof(OrderExample), typeof(OrderExample))]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            var result = await OrderRepository.CreateOrder(order);
            if (result == null)
            {
                Logger.LogError($"Order already exist.");
                return Conflict();
            }

            await OrderService.CreateOrder(order);
            return CreatedAtRoute("GetOrders", new { customerId = order.CustomerId}, order);
        }

        private readonly IOrderService OrderService;
        private readonly IOrderRepository OrderRepository;
        private readonly ILogger<OrderController> Logger;
    }
}