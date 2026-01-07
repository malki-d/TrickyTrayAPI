using Microsoft.AspNetCore.Mvc;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class PurchaseController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchaseController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetPurchases()
    {
        var purchases = await _purchaseService.GetAllAsync();
        return Ok(purchases);
    }
    [HttpGet("revenue")]
    public async Task<ActionResult<PurchaseRevenueDTO>> GetTotalRevenue()
    {
        var revenue = await _purchaseService.GetTotalRevenueAsync();
        return Ok(revenue);
    }

    [HttpPost("checkout/{userId}")]
    public async Task<IActionResult> Checkout(int userId)
    {
        try
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid User ID");
            }

            // העברת ה-ID לשכבת הסרוויס
            var purchase = await _purchaseService.ProcessPurchaseAsync(userId);

            return Ok(new
            {
                Message = "Purchase successful",
                PurchaseId = purchase.Id,
                UserId = purchase.UserId,
                TotalTickets = purchase.PurchaseItems.Count, // סך הכרטיסים שנוצרו
                TotalPrice = purchase.Price
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}