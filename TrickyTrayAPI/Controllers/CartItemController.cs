using Microsoft.AspNetCore.Mvc;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Services;
using TrickyTrayAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace TrickyTrayAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartItemService _service;

        public CartItemsController(ICartItemService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCartItemDTO>>> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetCartItemDTO>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }
        [HttpGet("user/{id}")]
        public async Task<ActionResult<IEnumerable<GetCartItemWithGiftDTO>>> GetByUserId(int id)
        {
            var items = await _service.GetAllUserCartAsync(id);
            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult<GetCartItemDTO>> Create([FromBody] CreateCartItemDTO dto)
        {
            
            var created = await _service.CreateAsync(dto);

            if (created == null)
                return BadRequest("this gift was random, you cant buy this...");


            return CreatedAtAction(nameof(GetById), new { id = created.Id },new GetCartItemDTO() { UserName=created.User.FirstName,GiftName=created.Gift.Name,Quantity=created.Quantity});
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCartItemDTO dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}