using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth.Request;

public class VerifyOtpRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Verification code is required.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Verification code must be a 6-digit number.")]
    public string Code { get; set; } = null!;
}
