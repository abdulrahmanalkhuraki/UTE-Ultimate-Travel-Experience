using System.ComponentModel.DataAnnotations;
using Application.Common.Validation;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
    public string LastName { get; set; } = null!;

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

    [Required(ErrorMessage = "Date of birth is required.")]
    [DateOfBirth(MinAge = 18, MaxAge = 120)]
    [DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; }

    [RegularExpression(@"^[\+]?[\d\s\-\(\)]{6,20}$",
        ErrorMessage = "Phone number format is invalid.")]
    [StringLength(20)]
    public string? Phone { get; set; }

    public IFormFile? Image { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Role must be a positive number.")]
    public int Role { get; set; }
}
