using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
    //[RegularExpression(@"^[A-Za-z0-9_\.]+$",
    //    ErrorMessage = "Username can only contain letters, digits, dot and underscore.")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    [StringLength(75, ErrorMessage = "Email must be at most 75 characters.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one digit.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Password confirmation is required.")]
    [Compare(nameof(Password), ErrorMessage = "Password and confirmation do not match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = null!;

    [RegularExpression(@"^[\+]?[\d\s\-\(\)]{6,20}$",
        ErrorMessage = "Phone number format is invalid.")]
    [StringLength(20)]
    public string? Phone { get; set; }

   // [Url(ErrorMessage = "Image must be a valid URL.")]
    [StringLength(500)]
    public string? Image { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Role must be a positive number.")]
    public int Role { get; set; }
}
