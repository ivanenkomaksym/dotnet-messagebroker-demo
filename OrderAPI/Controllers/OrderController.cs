using Common.Examples;
using Common.Models;
using Common.Models.Payment;
using Microsoft.AspNetCore.Mvc;
using OrderCommon.Repositories;
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

        [HttpPut("{orderId}/Payment")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdatePayment(Guid orderId, [FromBody] PaymentInfo paymentInfo)
        {
            var result = await OrderRepository.UpdatePayment(orderId, paymentInfo);
            if (!result)
            {
                Logger.LogError($"Failed to update payment.");
                return BadRequest();
            }
            var order = await OrderRepository.GetOrderById(orderId);
            await OrderService.UpdateOrder(order);

            return Ok(result);
        }

        [HttpPost("{orderId}/Cancel")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Cancel(Guid orderId)
        {
            var result = await OrderRepository.Cancel(orderId);
            if (!result)
            {
                Logger.LogError($"Failed to cancel order.");
                return BadRequest();
            }
            var order = await OrderRepository.GetOrderById(orderId);
            await OrderService.UpdateOrder(order);

            return Ok(result);
        }

        [HttpPost("{orderId}/Collected")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Collected(Guid orderId)
        {
            var result = await OrderRepository.Collected(orderId);
            if (!result)
            {
                Logger.LogError($"Failed to set Collected on order.");
                return BadRequest();
            }
            var order = await OrderRepository.GetOrderById(orderId);
            await OrderService.UpdateOrder(order);

            return Ok(result);
        }

        private readonly IOrderService OrderService;
        private readonly IOrderRepository OrderRepository;
        private readonly ILogger<OrderController> Logger;
    }
}