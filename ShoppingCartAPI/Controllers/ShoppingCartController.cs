using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.Entities;
using ShoppingCartAPI.Repositories;
using System.Net;

namespace ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ILogger<ShoppingCartController> _logger;

        public ShoppingCartController(IShoppingCartRepository shoppingCartRepository, ILogger<ShoppingCartController> logger)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _logger = logger;
        }

        [HttpGet("{customerId}", Name = "GetShoppingCart")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> Get(Guid customerId)
        {
            var shoppingCart = await _shoppingCartRepository.GetShoppingCart(customerId);

            if (shoppingCart == null)
            {
                _logger.LogError($"Shopping cart with customerId: {customerId}, not found.");
                return NotFound();
            }

            return Ok(shoppingCart);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateShoppingCart([FromBody] ShoppingCart cart)
        {
            await _shoppingCartRepository.CreateShoppingCart(cart);

            return CreatedAtRoute("GetShoppingCart", new { customerId = cart.CustomerId }, cart);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateShoppingCart([FromBody] ShoppingCart cart)
        {
            return Ok(await _shoppingCartRepository.UpdateShoppingCart(cart));
        }

        [HttpDelete("{customerId}")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProductById(Guid customerId)
        {
            return Ok(await _shoppingCartRepository.DeleteShoppingCart(customerId));
        }


        [HttpPost("Checkout")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> Checkout(Guid customerId)
        {
            return await _shoppingCartRepository.Checkout(customerId);
        }
    }
}