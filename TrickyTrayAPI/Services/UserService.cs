using TrickyTrayAPI.DTOs;

using TrickyTrayAPI.Repositories;
using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IConfiguration configuration,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToResponseDto);
    }

    public async Task<UserResponseDTO?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null ? MapToResponseDto(user) : null;
    }

    public async Task<UserResponseDTO> CreateUserAsync(UserCreateDTO createDto)
    {
        if (await _userRepository.EmailExistsAsync(createDto.Email))
        {
            throw new ArgumentException($"Email {createDto.Email} is already registered.");
        }

        // Note: In production, use proper password hashing (e.g., BCrypt, Argon2)
        var user = new User
        {
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Email = createDto.Email,
            PasswordHash = HashPassword(createDto.Password), // Simplified - use proper hashing
            PhoneNumber = createDto.Phone,
            TypeCostumer = TypeCostumer.User
        };

        var createdUser = await _userRepository.CreateAsync(user);
        _logger.LogInformation("User created with ID: {UserId}", createdUser.Id);

        return MapToResponseDto(createdUser);
    }

    public async Task<UserResponseDTO?> UpdateUserAsync(int id, UserUpdateDTO updateDto)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null) return null;

        if (updateDto.Email != null && updateDto.Email != existingUser.Email)
        {
            if (await _userRepository.EmailExistsAsync(updateDto.Email))
            {
                throw new ArgumentException($"Email {updateDto.Email} is already registered.");
            }
            existingUser.Email = updateDto.Email;
        }

        if (updateDto.FirstName != null) existingUser.FirstName = updateDto.FirstName;
        if (updateDto.LastName != null) existingUser.LastName = updateDto.LastName;
        if (updateDto.Phone != null) existingUser.PhoneNumber = updateDto.Phone;

        var updatedUser = await _userRepository.UpdateAsync(existingUser);
        return updatedUser != null ? MapToResponseDto(updatedUser) : null;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        return await _userRepository.DeleteAsync(id);
    }

    public async Task<LoginResponseDTO?> AuthenticateAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            _logger.LogWarning("Login attempt failed: User not found for email {Email}", email);
            return null;
        }

        // Verify password (simplified - in production use proper password verification)
        var hashedPassword = HashPassword(password);
        if (user.PasswordHash != hashedPassword)
        {
            _logger.LogWarning("Login attempt failed: Invalid password for email {Email}", email);
            return null;
        }

        var token = _tokenService.GenerateToken(user.Id, user.Email, user.FirstName, user.LastName, user.TypeCostumer);
        var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes", 60);

        _logger.LogInformation("User {UserId} authenticated successfully", user.Id);

        return new LoginResponseDTO
        {
            Token = token,
            TokenType = "Bearer",
            ExpiresIn = expiryMinutes * 60, // Convert to seconds
            User = MapToResponseDto(user)
        };
    }

    private static UserResponseDTO MapToResponseDto(User user)
    {
        return new UserResponseDTO
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.PhoneNumber,
            Type = user.TypeCostumer.ToString()
        };
    }

    // Simplified password hashing - In production, use BCrypt, Argon2, or Identity framework
    private static string HashPassword(string password)
    {
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }
}