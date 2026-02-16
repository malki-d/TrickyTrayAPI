using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class PurchaseController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;
    private readonly ILogger<PurchaseController> _logger;

    public PurchaseController(IPurchaseService purchaseService, ILogger<PurchaseController> logger)
    {
        _purchaseService = purchaseService;
        _logger = logger;
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
            _logger.LogError(ex, "Error while getting all purchases");

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "שגיאה בשרת",
                Detail = "אירעה שגיאה בעת ניסיון להביא את רשימת הרכישות. נסה/י שוב מאוחר יותר."
            };

            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }
    [HttpGet("revenue")]
    public async Task<ActionResult<PurchaseRevenueDTO>> GetTotalRevenue()
    {
        try
        {
            var revenue = await _purchaseService.GetTotalRevenueAsync();
            return Ok(revenue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting total revenue");

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "שגיאה בשרת",
                Detail = "אירעה שגיאה בעת ניסיון לחשב את סך ההכנסות. נסה/י שוב מאוחר יותר."
            };

            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }

    [HttpPost("checkout/{userId}")]
    public async Task<IActionResult> Checkout(int userId)
    {
        try
        {
            if (userId <= 0)
            {
                var validationProblem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "בקשה לא תקינה",
                    Detail = "מזהה המשתמש שסופק אינו תקין."
                };

                return BadRequest(validationProblem);
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
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business error during checkout for user {UserId}", userId);

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "לא ניתן להשלים את הרכישה",
                Detail = ex.Message
            };

            return BadRequest(problem);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error during checkout for user {UserId}", userId);

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "שגיאה במסד הנתונים",
                Detail = "אירעה שגיאה במסד הנתונים במהלך תהליך הרכישה. נסה/י שוב מאוחר יותר."
            };

            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during checkout for user {UserId}", userId);

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "שגיאה בשרת",
                Detail = "אירעה שגיאה בעת ניסיון להשלים את הרכישה. נסה/י שוב מאוחר יותר."
            };

            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    }
    [HttpGet("ByUser/{userId}")]
    public async Task<IActionResult> GetPurchasesByUserId(int userId)
    {
        // 1. בדיקת תקינות בסיסית (Validation)
        if (userId <= 0)
        {
            var validationProblem = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "בקשה לא תקינה",
                Detail = "מזהה המשתמש שסופק אינו תקין."
            };

            return BadRequest(validationProblem);
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
            _logger.LogError(ex, "Error while getting purchases for user {UserId}", userId);

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "שגיאה בשרת",
                Detail = "אירעה שגיאה בעת ניסיון להביא את הרכישות של המשתמש. נסה/י שוב מאוחר יותר."
            };

            return StatusCode(StatusCodes.Status500InternalServerError, problem);
        }
    
}
}