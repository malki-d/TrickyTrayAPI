using Microsoft.AspNetCore.Mvc;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Services;

namespace TrickyTrayAPI.Controllers
{
    [ApiController]
    [Route("api/ticket-price")]
    public class TicketPriceController : ControllerBase
    {
        private readonly ITicketPriceService _ticketPriceService;
        private readonly ILogger<TicketPriceController> _logger; // הוספת הלוגר

        public TicketPriceController(ITicketPriceService ticketPriceService, ILogger<TicketPriceController> logger)
        {
            _ticketPriceService = ticketPriceService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<GetTicketPriceDTO>> Get()
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve ticket price.");

                var price = await _ticketPriceService.GetAsync();

                if (price == null)
                {
                    _logger.LogWarning("Ticket price not found in the database.");
                    return NotFound("Ticket price is not configured.");
                }

                return Ok(price);
            }
            catch (Exception ex)
            {
                // כתיבת השגיאה המלאה ללוג כולל ה-Stack Trace
                _logger.LogError(ex, "An error occurred while fetching the ticket price.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpPut]
        public async Task<ActionResult<GetTicketPriceDTO>> Update([FromBody] UpdateTicketPriceDTO dto)
        {
            try
            {
                if (dto.Price <= 0)
                {
                    _logger.LogWarning("Update failed: Price provided ({Price}) is invalid.", dto.Price);
                    return BadRequest("Price must be greater than zero");
                }

                _logger.LogInformation("Updating ticket price to: {Price}", dto.Price);

                var updated = await _ticketPriceService.UpdateAsync(dto.Price);

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the ticket price to {Price}.", dto.Price);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}