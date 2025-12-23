using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrickyTrayAPI.Models;
using TrickyTrayAPI.Repositories;
using TrickyTrayAPI.Services;

namespace TrickyTrayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        private readonly IGiftService _giftservice;


        public GiftController(IGiftService giftservice)
        {
            _giftservice = giftservice;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gift>>> GetAll()
        {
            var gifts = await _giftservice.GetAllAsync();
            return Ok(gifts);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Gift>> GetById(int id)
        {
            var gift = await _giftservice.GetByIdAsync(id);

            if (gift == null)
                return NotFound();

            return Ok(gift);
        }


        [HttpPost]
        public async Task<ActionResult<Gift>> Create(Gift gift)
        {
            var createdGift = await _giftservice.AddAsync(gift);
            //return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
            //return Ok(createdGift);
            return CreatedAtAction(nameof(GetById), new { id = createdGift.Id }, createdGift);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Gift>> Update(int id, Gift gift)
        {
            if (id != gift.Id)
                return BadRequest();

            var exists = await _giftservice.ExistsAsync(id);
            if (!exists)
                return NotFound();

            var updatedProduct = await _giftservice.UpdateAsync(gift);
            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _giftservice.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
