using Google.Apis.Auth;
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
        private readonly string _googleClientId;

        public AuthController(
            IUserService userService,
            ILogger<AuthController> logger,
            IConfiguration configuration)
        {
            _userService = userService;
            _logger = logger;
            _googleClientId = configuration["Authentication:Google:ClientId"]
                              ?? throw new InvalidOperationException("Google ClientId is not configured");
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
            /// Authenticate user using Google ID token and get JWT token
            /// </summary>
            [HttpPost("google")]
            [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public async Task<ActionResult<LoginResponseDTO>> GoogleLogin([FromBody] GoogleLoginRequestDTO request)
            {
                if (string.IsNullOrWhiteSpace(request.IdToken))
                {
                    return BadRequest(new { message = "idToken is required." });
                }

                try
                {
                    var validationSettings = new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { _googleClientId }
                    };

                    var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, validationSettings);

                    if (payload == null || string.IsNullOrWhiteSpace(payload.Email))
                    {
                        _logger.LogWarning("Google login failed: invalid payload or missing email");
                        return Unauthorized(new { message = "Invalid Google token payload." });
                    }

                    var googleId = payload.Subject; // מזהה ייחודי של המשתמש בגוגל ("sub")

                    var result = await _userService.AuthenticateWithGoogleAsync(googleId, payload.Email, payload.GivenName, payload.FamilyName);

                    if (result == null)
                    {
                        _logger.LogWarning("Google login failed: could not authenticate or create user for email {Email}", payload.Email);
                        return Unauthorized(new { message = "Unable to authenticate user with Google account." });
                    }

                    return Ok(result);
                }
                catch (InvalidJwtException ex)
                {
                    _logger.LogWarning(ex, "Google login failed: invalid JWT");
                    return Unauthorized(new { message = "Invalid Google token." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Google login failed due to an unexpected error");
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while validating Google token." });
                }
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

