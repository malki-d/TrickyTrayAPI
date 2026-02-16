using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Services;

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
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא את פריטי הרכישה. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
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
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "לא נמצא",
                        Detail = $"לא נמצא פריט רכישה עם מזהה {id}."
                    });
                }
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in GetById purchase item: {Id}", id);
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא את פריט הרכישה. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
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
                    return BadRequest(new ValidationProblemDetails(ModelState)
                    {
                        Title = "הנתונים שנשלחו אינם תקינים"
                    });
                }

                var newItem = await _service.AddAsync(purchaseItem);
                return CreatedAtAction(nameof(GetById), new { id = newItem.Id }, newItem);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error in Create purchase item");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה במסד הנתונים",
                    Detail = "אירעה שגיאה בעת שמירת פריט הרכישה במסד הנתונים. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in Create purchase item");
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת יצירת פריט רכישה חדש. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
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
                    return BadRequest(new ValidationProblemDetails(ModelState)
                    {
                        Title = "הנתונים שנשלחו אינם תקינים"
                    });
                }

                var exists = await _service.ExistsAsync(id);
                if (!exists)
                {
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "לא נמצא",
                        Detail = $"לא נמצא פריט רכישה עם מזהה {id} לעדכון."
                    });
                }

                var updatedItem = await _service.UpdateAsync(purchaseItem, id);
                return Ok(updatedItem);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error in Update purchase item: {Id}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה במסד הנתונים",
                    Detail = "אירעה שגיאה בעת עדכון פריט הרכישה במסד הנתונים. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in Update purchase item: {Id}", id);
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת עדכון פריט הרכישה. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
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
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "לא נמצא",
                        Detail = $"לא נמצא פריט רכישה עם מזהה {id} למחיקה."
                    });
                }

                var result = await _service.DeleteAsync(id);
                if (result)
                {
                    return NoContent();
                }
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "מחיקה נכשלה",
                    Detail = "לא ניתן היה למחוק את פריט הרכישה."
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error in Delete purchase item: {Id}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה במסד הנתונים",
                    Detail = "אירעה שגיאה בעת מחיקת פריט הרכישה במסד הנתונים. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in Delete purchase item: {Id}", id);
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת מחיקת פריט הרכישה. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
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
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא את פריטי הרכישה עבור מתנה זו. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
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
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא את פריטי הרכישה עבור משתמש זה. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }
    }
}
