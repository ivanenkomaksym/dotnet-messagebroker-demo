using System.Net;
using Common.Examples;
using Common.Models;
using Common.Models.Payment;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.Messaging;
using OrderCommon.Repositories;
using Swashbuckle.AspNetCore.Filters;

namespace OrderAPI.Controllers
{
    /// <summary>
    /// Orders controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        /// <summary>
        /// Initializes controller.
        /// </summary>
        /// <param name="orderPublisher">Used for publishing messages.</param>
        /// <param name="orderRepository">Repository for orders data.</param>
        /// <param name="logger">Used for logging.</param>
        public OrderController(IOrderPublisher orderPublisher, IOrderRepository orderRepository, ILogger<OrderController> logger)
        {
            OrderPublisher = orderPublisher;
            OrderRepository = orderRepository;
            Logger = logger;
        }

        /// <summary>
        /// Get all orders.
        /// </summary>
        /// <returns>List of orders.</returns>
        [HttpGet(Name = "GetOrders")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Order>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await OrderRepository.GetAllOrders();

            return Ok(orders);
        }

        /// <summary>
        /// Get Order by id.
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>Order if found.</returns>
        [HttpGet("{orderId}", Name = "GetOrder")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Order>> GetOrderById(Guid orderId)
        {
            var order = await OrderRepository.GetOrderById(orderId);

            if (order == null)
            {
                Logger.LogError($"Order with id: {orderId} not found.");
                return NotFound();
            }

            return Ok(order);
        }

        /// <summary>
        /// Get orders by customer id.
        /// </summary>
        /// <param name="customerId">Customer id.</param>
        /// <returns>List of orders of a certain customer.</returns>
        [Route("[action]/{customerId}", Name = "GetOrdersByCustomerId")]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetOrdersByCustomerId(Guid customerId)
        {
            return Ok(await OrderRepository.GetOrdersByCustomerId(customerId));
        }

        /// <summary>
        /// Create new order.
        /// </summary>
        /// <param name="order">New order.</param>
        /// <returns>Order if created.</returns>
        [HttpPost(Name = "CreateOrder")]
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

            await OrderPublisher.CreateOrder(order);
            return CreatedAtRoute("GetOrdersByCustomerId", new { customerId = order?.CustomerInfo?.CustomerId }, order);
        }

        /// <summary>
        /// Update order's payment.
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="paymentInfo">New payment info.</param>
        /// <returns>True if updated.</returns>
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
            await OrderPublisher.UpdateOrder(order);

            return Ok(result);
        }

        /// <summary>
        /// Cancel order.
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>True if cancelled.</returns>
        [HttpPost("{orderId}/Cancel")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Cancel(Guid orderId)
        {
            var order = await OrderRepository.GetOrderById(orderId);

            if (order == null)
            {
                Logger.LogError($"Order with id: {orderId} not found.");
                return NotFound();
            }

            await OrderPublisher.CancelOrder(orderId);

            return Ok(true);
        }

        /// <summary>
        /// Confirm order's collection.
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>True if accepted.</returns>
        [HttpPost("{orderId}/Collected")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Collected(Guid orderId)
        {
            var order = await OrderRepository.GetOrderById(orderId);

            if (order == null)
            {
                Logger.LogError($"Order with id: {orderId} not found.");
                return NotFound();
            }

            await OrderPublisher.OrderCollected(orderId);

            return Ok(true);
        }

        /// <summary>
        /// Return order.
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>True if accepted.</returns>
        [HttpPost("{orderId}/Return")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Return(Guid orderId)
        {
            var order = await OrderRepository.GetOrderById(orderId);

            if (order == null)
            {
                Logger.LogError($"Order with id: {orderId} not found.");
                return NotFound();
            }

            await OrderPublisher.ReturnOrder(orderId);

            return Ok(true);
        }

        private readonly IOrderPublisher OrderPublisher;
        private readonly IOrderRepository OrderRepository;
        private readonly ILogger<OrderController> Logger;
    }
}