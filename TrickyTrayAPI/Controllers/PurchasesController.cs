using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Services;

namespace TrickyTrayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _service;

        public PurchasesController(IPurchaseService service)
        {
            _service = service;
        }

        // GET: api/Purchases
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetPurchaseDTO>>> GetPurchases()
        {
            var purchases = await _service.GetAllAsync();
            return Ok(purchases);
        }

        // GET: api/Purchases/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetPurchaseDTO>> GetPurchase(int id)
        {
            var purchase = await _service.GetByIdAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }
            return Ok(purchase);
        }

        // POST: api/Purchases
        [HttpPost]
        public async Task<ActionResult<GetPurchaseDTO>> PostPurchase(CreatePurchaseDTO dto)
        {
            var created = await _service.AddAsync(dto);
            // If you have an Id property in GetPurchaseDTO, you can use it in CreatedAtAction
            return Ok(created);
        }

        // PUT: api/Purchases/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchase(int id, CreatePurchaseDTO dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE: api/Purchases/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        // GET: api/Purchases/revenue
        [HttpGet("revenue")]
        public async Task<ActionResult<PurchaseRevenueDTO>> GetTotalRevenue()
        {
            var revenue = await _service.GetTotalRevenueAsync();
            return Ok(revenue);
        }
    }
}