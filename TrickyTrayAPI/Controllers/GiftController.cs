using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrickyTrayAPI.DTOs;
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
        public async Task<ActionResult<IEnumerable<GetGiftDTO>>> GetAll()
        {
            var gifts = await _giftservice.GetAllAsync();
            return Ok(gifts);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<GetGiftDTO>> GetById(int id)
        {
            var gift = await _giftservice.GetByIdAsync(id);

            if (gift == null)
                return NotFound();

            return Ok(gift);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Gift>> Create(CreateGiftDTO gift)
        {
            var createdGift = await _giftservice.AddAsync(gift);
            return Ok(createdGift);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<Gift>> Update(UpdateGiftDTO gift,int id)
        {

            var exists = await _giftservice.ExistsAsync(id);
            if (!exists)
                return NotFound();

            var updatedProduct = await _giftservice.UpdateAsync(gift,id);
            return Ok(updatedProduct);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _giftservice.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<GetGiftDTO>>> Search([FromQuery] string? giftName, [FromQuery] string? donorName, [FromQuery] int? purchaserCount)
        {
            var results = await _giftservice.SearchAsync(giftName, donorName, purchaserCount);
            return Ok(results);
        }
        [HttpGet("sorted")]
        public async Task<ActionResult<IEnumerable<GetGiftDTO>>> GetSorted([FromQuery] bool sortByName = false, [FromQuery] bool sortByCategory = false)
        {
            var gifts = await _giftservice.GetSortedAsync(sortByName, sortByCategory);
            return Ok(gifts);
        }
        [HttpGet("by-category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<GetGiftDTO>>> GetByCategory(int categoryId)
        {
            var gifts = await _giftservice.GetByCategoryAsync(categoryId);
            return Ok(gifts);
        }
        [Authorize]
        [HttpGet("random")]
        public async Task<ActionResult<IEnumerable<GetGiftWithWinnerDTO>>> RandomWinners()
        {
            var gifts = await _giftservice.RandomWinners();
            return Ok(gifts);
        }

        [HttpGet("winners-report")]
        public async Task<ActionResult<IEnumerable<GiftWinnerReportDTO>>> GetWinnersReport()
        {
            var report = await _giftservice.GetGiftWinnersReportAsync();
            return Ok(report);
        }
        [Authorize]
        [HttpGet("winners-report/export")]
        public async Task<IActionResult> ExportWinnersReport([FromQuery] string format = "csv")
        {
            if (string.Equals(format, "excel", StringComparison.OrdinalIgnoreCase))
            {
                var bytes = await _giftservice.ExportWinnersReportExcelAsync();
                return File(bytes, "application/vnd.ms-excel", "winners-report.xls");
            }

            // default csv
            var csv = await _giftservice.ExportWinnersReportCsvAsync();
            return File(csv, "text/csv", "winners-report.csv");
        }

        [HttpGet("by-purchases/{sortBy}")]
        public async Task<ActionResult<IEnumerable<GetGiftDTO>>> GetSortedGiftsAsync(string sortBy)
        {
            var gifts = await _giftservice.GetSortedGiftsAsync(sortBy);
            return Ok(gifts);
        }

    }
}
