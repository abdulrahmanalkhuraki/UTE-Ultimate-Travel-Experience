using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth.Request;

public class ResetPasswordRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    [StringLength(75, ErrorMessage = "Email must be at most 75 characters.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Verification code is required.")]
    [StringLength(10, MinimumLength = 4, ErrorMessage = "Code must be between 4 and 10 characters.")]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "New password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one digit.")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage = "Password confirmation is required.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Password and confirmation do not match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = null!;
}
