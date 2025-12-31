using Microsoft.AspNetCore.Mvc;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Services;

namespace TrickyTrayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
      
       
            private readonly IUserService _userService;
            private readonly ILogger<AuthController> _logger;

            public AuthController(
                IUserService userService,
                ILogger<AuthController> logger)
            {
                _userService = userService;
                _logger = logger;
            }

            /// <summary>
            /// Authenticate user and get JWT token
            /// </summary>
            [HttpPost("login")]
            [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO loginDto)
            {
                if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    return BadRequest(new { message = "Email and password are required." });
                }

                var result = await _userService.AuthenticateAsync(loginDto.Email, loginDto.Password);

                if (result == null)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                return Ok(result);
            }

            /// <summary>
            /// Register a new user
            /// </summary>
            [HttpPost("register")]
            [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status201Created)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public async Task<ActionResult<UserResponseDTO>> Register([FromBody] UserCreateDTO createDto)
            {
                try
                {
                    var user = await _userService.CreateUserAsync(createDto);
                    return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
        }
    }

