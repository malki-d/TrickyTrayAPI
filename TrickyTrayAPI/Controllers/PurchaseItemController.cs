using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace TrickyTrayAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseItemController : ControllerBase
    {
        private readonly IPurchaseItemService _service;
        private readonly ILogger<PurchaseItemController> _logger;

        public PurchaseItemController(IPurchaseItemService service, ILogger<PurchaseItemController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/PurchaseItem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetPurchaseItemDTO>>> GetAll()
        {
            try
            {
                var items = await _service.GetAllAsync();
                return Ok(items);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll purchase items");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/PurchaseItem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetPurchaseItemDTO>> GetById(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                if (item == null)
                {
                    return NotFound($"Purchase item with id {id} not found");
                }
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in GetById purchase item: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/PurchaseItem
        [HttpPost]
        public async Task<ActionResult<GetPurchaseItemDTO>> Create([FromBody] CreatePurchaseItemDTO purchaseItem)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newItem = await _service.AddAsync(purchaseItem);
                return CreatedAtAction(nameof(GetById), new { id = newItem.Id }, newItem);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in Create purchase item");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/PurchaseItem/5
        [HttpPut("{id}")]
        public async Task<ActionResult<GetPurchaseItemDTO>> Update(int id, [FromBody] UpdatePurchaseItemDTO purchaseItem)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var exists = await _service.ExistsAsync(id);
                if (!exists)
                {
                    return NotFound($"Purchase item with id {id} not found");
                }

                var updatedItem = await _service.UpdateAsync(purchaseItem, id);
                return Ok(updatedItem);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in Update purchase item: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/PurchaseItem/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var exists = await _service.ExistsAsync(id);
                if (!exists)
                {
                    return NotFound($"Purchase item with id {id} not found");
                }

                var result = await _service.DeleteAsync(id);
                if (result)
                {
                    return NoContent();
                }
                return BadRequest("Failed to delete purchase item");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in Delete purchase item: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/PurchaseItem/gift/5
        [HttpGet("gift/{giftId}")]
        public async Task<ActionResult<IEnumerable<GetPurchaseItemDTO>>> GetByGift(int giftId)
        {
            try
            {
                var items = await _service.GetPurchaseItemsForGiftAsync(giftId);
                return Ok(items);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in GetByGift purchase items: {GiftId}", giftId);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/PurchaseItem/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<GetPurchaseItemDTO>>> GetByUser(int userId)
        {
            try
            {
                var items = await _service.GetPurchaseItemsForUserAsync(userId);
                return Ok(items);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in GetByUser purchase items: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
