using System.ComponentModel.DataAnnotations;

namespace TrickyTrayAPI.DTOs;

using System.ComponentModel.DataAnnotations;

public class UserCreateDTO
{
    [Required(ErrorMessage = "שם פרטי הוא שדה חובה")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "שם פרטי חייב להיות בין 2 ל-50 תווים")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "שם משפחה הוא שדה חובה")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "שם משפחה חייב להיות בין 2 ל-50 תווים")]
    public string LastName { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "פורמט האימייל אינו תקין")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "הסיסמה חייבת להכיל לפחות 6 תווים")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "הסיסמה חייבת לכלול אות גדולה, אות קטנה ומספר")]
    public string Password { get; set; }

    [Phone(ErrorMessage = "מספר הטלפון אינו תקין")]
    public string Phone { get; set; }
}

public class UserUpdateDTO
{
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? Phone { get; set; }

}

public class UserResponseDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Type { get; set; }
}