using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth.Request;

public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    [StringLength(75, ErrorMessage = "Email must be at most 75 characters.")]
    public string Email { get; set; } = null!;
}
