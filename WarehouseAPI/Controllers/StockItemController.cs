using System.Net;
using Common.Models.Warehouse;
using WarehouseCommon.Repositories;
using DnsClient.Internal;
using Microsoft.AspNetCore.Mvc;
using Common.Models;

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

        [HttpPost]
        [ProducesResponseType(typeof(StockItem), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<StockItem>> CreateStockItem(StockItem stockItem)
        {
            var created = await _repository.CreateStockItem(stockItem);

            return Ok(created);
        }

        [HttpPut]
        [ProducesResponseType(typeof(StockItem), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<StockItem>> UpdateStockItem(StockItem stockItem)
        {
            var updated = await _repository.UpdateStockItem(stockItem);

            return Ok(updated);
        }


        [Route("[action]/{productId}", Name = "DeleteStockItemByProductId")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> DeleteStockItemByProductId(Guid productId)
        {
            var stockItem = await _repository.GetStockItemByProductId(productId);

            if (stockItem == null)
            {
                _logger.LogError($"StockItem with product id: {productId}, not found.");
                return NotFound();
            }

            return Ok(await _repository.DeleteStockItem(stockItem.Id));
        }
    }
}
