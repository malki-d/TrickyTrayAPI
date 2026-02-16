using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;
using TrickyTrayAPI.Services;

namespace TrickyTrayAPI.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DonorsController : ControllerBase
    {
        private readonly IDonorService _donorservice;
        private readonly ILogger<DonorsController> _logger;

        public DonorsController(IDonorService donorservice, ILogger<DonorsController> logger)
        {
            _donorservice = donorservice;
            _logger = logger;
        }
        //פונקציה שמביאה כמה תורמים יש לי בסך הכל



        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetDonorDTO>>> GetAll()
        {
            try
            {
                var donors = await _donorservice.GetAllDonors();
                return Ok(donors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all donors");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא את רשימת התורמים. נסה שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CreateDonorDTO>> GetById(int id)
        {
            try
            {
                var donor = await _donorservice.GetDonorById(id);

                if (donor == null)
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "תורם לא נמצא",
                        Detail = $"לא נמצא תורם עם מזהה {id}."
                    });

                return Ok(donor);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while getting donor by id {DonorId}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "בקשה שגויה",
                    Detail = "בקשתך אינה תקינה. בדוק את הנתונים שנשלחו ונסה שוב."
                };

                return BadRequest(problem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting donor by id {DonorId}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא את פרטי התורם. נסה שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Donor>> Create(CreateDonorDTO donor)
        {
            try
            {
                var createdDonor = await _donorservice.AddDonor(donor);
                return CreatedAtAction(nameof(GetById), new { id = createdDonor.Id }, createdDonor);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating donor");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "נתונים שגויים",
                    Detail = "חלק מהנתונים שנשלחו אינם תקינים. בדוק ונסה שוב."
                };

                return BadRequest(problem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating donor");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון ליצור תורם חדש. נסה שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GetDonorDTO>> Update(int id, CreateDonorDTO donor)
        {
            try
            {
                var exists = await _donorservice.ExistsAsync(id);
                if (!exists)
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "תורם לא נמצא",
                        Detail = $"לא ניתן לעדכן תורם – לא נמצא תורם עם מזהה {id}."
                    });

                var updatedDonor = await _donorservice.UpdateAsync(id, donor);
                return Ok(updatedDonor);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating donor {DonorId}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "נתונים שגויים",
                    Detail = "חלק מהנתונים שנשלחו לעדכון אינם תקינים. בדוק ונסה שוב."
                };

                return BadRequest(problem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating donor {DonorId}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון לעדכן את התורם. נסה שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _donorservice.DeleteDonor(id);

                if (!deleted)
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "תורם לא נמצא",
                        Detail = $"לא ניתן למחוק – לא נמצא תורם עם מזהה {id}."
                    });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting donor {DonorId}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון למחוק את התורם. נסה שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }
       

        [HttpGet("{donorId}/with-gifts")]
        public async Task<ActionResult<GetDonorWithGiftsDTO>> GetDonorWithGifts(int donorId)
        {
            try
            {
                var dto = await _donorservice.GetDonorWithGiftsAsync(donorId);

                if (dto == null)
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "תורם לא נמצא",
                        Detail = $"לא נמצא תורם עם מזהה {donorId} או שאין לו פריטים.",
                    });

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting donor with gifts {DonorId}", donorId);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא את פרטי התורם והפריטים שלו. נסה שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<GetDonorWithGiftsDTO>>> FilterDonors(
    [FromQuery] string? name,
    [FromQuery] string? email,
    [FromQuery] string? giftName)
        {
            try
            {
                var result = await _donorservice.FilterDonorsAsync(name, email, giftName);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while filtering donors");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "נתונים שגויים",
                    Detail = "חלק מהפרמטרים לפילטור אינם תקינים. בדוק ונסה שוב."
                };

                return BadRequest(problem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while filtering donors");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון לסנן תורמים. נסה שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }


    }
}

