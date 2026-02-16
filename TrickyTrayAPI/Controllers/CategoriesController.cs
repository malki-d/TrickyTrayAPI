using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrickyTrayAPI.Models;
using TrickyTrayAPI.Services;

namespace TrickyTrayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService service, ILogger<CategoriesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            try
            {
                var categories = await _service.GetAllAsync();
                return Ok(categories);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all categories");

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא את רשימת הקטגוריות. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            try
            {
                var category = await _service.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "קטגוריה לא נמצאה",
                        Detail = $"לא נמצאה קטגוריה עם מזהה {id}."
                    });
                }
                return Ok(category);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category {CategoryId}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון להביא את פרטי הקטגוריה. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }
        [Authorize]

        // POST: api/Categories
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    var validationProblem = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "שם קטגוריה נדרש",
                        Detail = "יש לספק שם תקין לקטגוריה."
                    };

                    return BadRequest(validationProblem);
                }

                var created = await _service.AddAsync(name);
                return CreatedAtAction(nameof(GetCategory), new { id = created!.Id }, created);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating category with name {CategoryName}", name);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון ליצור קטגוריה חדשה. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }
        [Authorize]

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, [FromBody] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    var validationProblem = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "בקשה לא תקינה",
                        Detail = "יש לספק שם תקין לקטגוריה."
                    };

                    return BadRequest(validationProblem);
                }

                var updated = await _service.UpdateAsync(id, name);
                if (!updated)
                {
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "קטגוריה לא נמצאה",
                        Detail = $"לא נמצאה קטגוריה עם מזהה {id} לעדכון."
                    });
                }

                return NoContent();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category {CategoryId}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון לעדכן את הקטגוריה. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }
        [Authorize]

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "קטגוריה לא נמצאה",
                        Detail = $"לא נמצאה קטגוריה עם מזהה {id} למחיקה."
                    });
                }
                return NoContent();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category {CategoryId}", id);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "שגיאה בשרת",
                    Detail = "אירעה שגיאה בעת ניסיון למחוק את הקטגוריה. נסה/י שוב מאוחר יותר."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }
    }
}