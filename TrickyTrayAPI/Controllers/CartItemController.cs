using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Services;
using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartItemService _service;
        private readonly ILogger<CartItemsController> _logger;

        public CartItemsController(ICartItemService service, ILogger<CartItemsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCartItemDTO>>> GetAll()
        {
            try
            {
                var items = await _service.GetAllAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting all cart items");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא את פרטי העגלה. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetCartItemDTO>> GetById(int id)
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
                        Detail = $"לא נמצא פריט עגלה עם מזהה {id}."
                    });
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting cart item by id {Id}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא את פריט העגלה. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }
        [HttpGet("user/{id}")]
        public async Task<ActionResult<IEnumerable<GetCartItemWithGiftDTO>>> GetByUserId(int id)
        {
            try
            {
                var items = await _service.GetAllUserCartAsync(id);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting cart items for user {UserId}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא את פריטי העגלה של המשתמש. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }

        [HttpPost]
        public async Task<ActionResult<GetCartItemDTO>> Create([FromBody] CreateCartItemDTO dto)
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

                var created = await _service.CreateAsync(dto);

                if (created == null)
                {
                    var problem = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "לא ניתן להוסיף לעגלה",
                        Detail = "המתנה כבר הוגרלה, לא ניתן לרכוש אותה."
                    };

                    return BadRequest(problem);
                }

                return CreatedAtAction(nameof(GetById), new { id = created.Id }, new GetCartItemDTO() { UserName = created.User.FirstName, GiftName = created.Gift.Name, Quantity = created.Quantity });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating cart item for user {UserId} and gift {GiftId}", dto.UserId, dto.GiftId);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה במסד הנתונים",
                    Detail = "אירעה שגיאה בעת שמירת פריט העגלה במסד הנתונים. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating cart item for user {UserId} and gift {GiftId}", dto.UserId, dto.GiftId);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת יצירת פריט עגלה חדש. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCartItemDTO dto)
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

                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "לא נמצא",
                        Detail = $"לא נמצא פריט עגלה עם מזהה {id} לעדכון."
                    });
                }

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating cart item {Id}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה במסד הנתונים",
                    Detail = "אירעה שגיאה בעת עדכון פריט העגלה במסד הנתונים. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating cart item {Id}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת עדכון פריט העגלה. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "לא נמצא",
                        Detail = $"לא נמצא פריט עגלה עם מזהה {id} למחיקה."
                    });
                }
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while deleting cart item {Id}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה במסד הנתונים",
                    Detail = "אירעה שגיאה בעת מחיקת פריט העגלה במסד הנתונים. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting cart item {Id}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת מחיקת פריט העגלה. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }
    }
}