using System.Net;
using Common.Models.Warehouse;
using WarehouseCommon.Repositories;
using DnsClient.Internal;
using Microsoft.AspNetCore.Mvc;

namespace WarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockItemController : ControllerBase
    {
        private readonly IWarehouseRepository _repository;
        private readonly ILogger<StockItemController> _logger;

        public StockItemController(IWarehouseRepository repository, ILogger<StockItemController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Route("[action]/{productId}", Name = "GetStockItemByProductId")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(StockItem), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<StockItem>> GetStockItemByProductId(Guid productId)
        {
            var stockItem = await _repository.GetStockItemByProductId(productId);

            if (stockItem == null)
            {
                _logger.LogError($"StockItem with product id: {productId}, not found.");
                return NotFound();
            }

            return Ok(stockItem);
        }
    }
}
