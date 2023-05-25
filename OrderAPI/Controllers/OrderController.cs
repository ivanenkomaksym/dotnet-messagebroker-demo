using Common.Examples;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.Repositories;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

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

        [HttpGet("{orderId}", Name = "GetOrder")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Order>> GetOrderById(Guid orderId)
        {
            var order = await OrderRepository.GetOrderById(orderId);

            if (order == null)
            {
                Logger.LogError($"Order with id: {orderId}, not found.");
                return NotFound();
            }

            return Ok(order);
        }

        [Route("[action]/{customerId}", Name = "GetOrdersByCustomerId")]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetOrdersByCustomerId(Guid customerId)
        {
            return Ok(await OrderRepository.GetOrdersByCustomerId(customerId));
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
            return CreatedAtRoute("GetOrdersByCustomerId", new { customerId = order.CustomerInfo.CustomerId}, order);
        }

        private readonly IOrderService OrderService;
        private readonly IOrderRepository OrderRepository;
        private readonly ILogger<OrderController> Logger;
    }
}