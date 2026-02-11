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
    public async Task<ActionResult<IEnumerable<GetPurchaseItemDTO>>> GetPurchases()
    {
        try
        {
            var purchases = await _purchaseService.GetAllAsync();
            return Ok(purchases);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "שגיאת שרת בעת שליפת כל הרכישות: " + ex.Message);
        }
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
    [HttpGet("ByUser/{userId}")]
    public async Task<IActionResult> GetPurchasesByUserId(int userId)
    {
        // 1. בדיקת תקינות בסיסית (Validation)
        if (userId <= 0)
        {
            return BadRequest("Invalid User ID provided.");
        }

        try
        {
            // 2. קריאה ל-BLL לקבלת הנתונים המעובדים
            var purchases = await _purchaseService.GetUserPurchasesLogic(userId);

            // 3. החזרת תשובה
            // גם אם הרשימה ריקה, מחזירים 200 OK עם מערך ריק (סטנדרט מקובל)
            return Ok(purchases);
        }
        catch (Exception ex)
        {
            // טיפול בשגיאות בלתי צפויות
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    
}
}