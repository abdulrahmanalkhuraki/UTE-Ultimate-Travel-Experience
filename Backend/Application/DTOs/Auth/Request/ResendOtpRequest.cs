using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth.Request;

public class ResendOtpRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    public string Email { get; set; } = null!;
}
