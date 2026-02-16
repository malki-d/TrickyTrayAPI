using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<GiftController> _logger;


        public GiftController(IGiftService giftservice, ILogger<GiftController> logger)
        {
            _giftservice = giftservice;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetGiftDTO>>> GetAll()
        {
            try
            {
                var gifts = await _giftservice.GetAllAsync();
                return Ok(gifts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all gifts");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא את רשימת המתנות. נסי שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<GetGiftDTO>> GetById(int id)
        {
            try
            {
                var gift = await _giftservice.GetByIdAsync(id);

                if (gift == null)
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "מתנה לא נמצאה",
                        Detail = $"לא נמצאה מתנה עם מזהה {id}."
                    });

                return Ok(gift);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while getting gift by id {GiftId}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "בקשה שגויה",
                    Detail = "בקשתך אינה תקינה. בדקי את הנתונים שנשלחו ונסי שוב."
                };

                return BadRequest(problem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting gift by id {GiftId}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא את פרטי המתנה. נסי שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }

        [HttpPost]
        public async Task<ActionResult<GetGiftDTO>> Create([FromForm] CreateGiftDTO gift)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState)
                {
                    Title = "הנתונים שנשלחו אינם תקינים"
                });
            }

            try
            {
                var createdGift = await _giftservice.AddAsync(gift);
                return Ok(createdGift);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Related entity missing while creating gift");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "נתונים שגויים",
                    Detail = ex.Message
                };

                return BadRequest(problem);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "File IO error while creating gift");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשמירת קובץ",
                    Detail = "אירעה שגיאה בעת שמירת קובץ התמונה של המתנה."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating gift");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון ליצור מתנה חדשה. נסי שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<GetGiftDTO>> Update([FromForm] UpdateGiftDTO gift, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState)
                {
                    Title = "הנתונים שנשלחו אינם תקינים"
                });
            }

            try
            {
                var exists = await _giftservice.ExistsAsync(id);
                if (!exists)
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "מתנה לא נמצאה",
                        Detail = $"לא נמצאה מתנה עם מזהה {id}."
                    });

                var updatedProduct = await _giftservice.UpdateAsync(gift, id);
                return Ok(updatedProduct);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Gift not found while updating {GiftId}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "נתונים שגויים",
                    Detail = ex.Message
                };

                return BadRequest(problem);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "File IO error while updating gift {GiftId}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשמירת קובץ",
                    Detail = "אירעה שגיאה בעת עדכון קובץ התמונה של המתנה."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating gift {GiftId}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון לעדכן את המתנה. נסי שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _giftservice.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "מתנה לא נמצאה",
                        Detail = $"לא נמצאה מתנה עם מזהה {id} או שלא ניתן למחוק אותה."
                    });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting gift {GiftId}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון למחוק את המתנה. נסי שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<GetGiftDTO>>> Search([FromQuery] string? giftName, [FromQuery] string? donorName, [FromQuery] int? purchaserCount)
        {
            try
            {
                var results = await _giftservice.SearchAsync(giftName, donorName, purchaserCount);
                return Ok(results);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while searching gifts");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "נתונים שגויים",
                    Detail = "חלק מהפרמטרים לחיפוש אינם תקינים. בדקי ונסי שוב."
                };

                return BadRequest(problem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching gifts");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון לחפש מתנות. נסי שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }
        [HttpGet("sorted")]
        public async Task<ActionResult<IEnumerable<GetGiftDTO>>> GetSorted([FromQuery] bool sortByName = false, [FromQuery] bool sortByCategory = false)
        {
            try
            {
                var gifts = await _giftservice.GetSortedAsync(sortByName, sortByCategory);
                return Ok(gifts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting sorted gifts");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא רשימת מתנות ממוינת. נסי שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }
        [HttpGet("by-category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<GetGiftDTO>>> GetByCategory(int categoryId)
        {
            try
            {
                var gifts = await _giftservice.GetByCategoryAsync(categoryId);
                return Ok(gifts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting gifts by category {CategoryId}", categoryId);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא מתנות לפי קטגוריה. נסי שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }
        [Authorize]
        [HttpGet("random")]
        public async Task<ActionResult<IEnumerable<GetGiftWithWinnerDTO>>> RandomWinners()
        {
            try
            {
                var gifts = await _giftservice.RandomWinners();
                return Ok(gifts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating random winners");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ביצוע ההגרלה. נסי שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }

        [HttpGet("winners-report")]
        public async Task<ActionResult<IEnumerable<GiftWinnerReportDTO>>> GetWinnersReport()
        {
            try
            {
                var report = await _giftservice.GetGiftWinnersReportAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting winners report");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא דו\"ח זוכים. נסי שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }
        [Authorize]
        [HttpGet("winners-report/export")]
        public async Task<IActionResult> ExportWinnersReport([FromQuery] string format = "csv")
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while exporting winners report. Format: {Format}", format);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת יצוא דו\"ח הזוכים. נסי שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }

        [HttpGet("by-purchases/{sortBy}")]
        public async Task<ActionResult<IEnumerable<GetGiftDTO>>> GetSortedGiftsAsync(string sortBy)
        {
            try
            {
                var gifts = await _giftservice.GetSortedGiftsAsync(sortBy);
                return Ok(gifts);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while getting sorted gifts by purchases");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "נתונים שגויים",
                    Detail = "פרמטר המיון שסופק אינו תקין."
                };

                return BadRequest(problem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting sorted gifts by purchases");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא מתנות ממוינות לפי רכישות. נסי שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }

    }
}
