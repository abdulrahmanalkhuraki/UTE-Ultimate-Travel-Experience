namespace Application.DTOs.Auth;

public class RegisterResponse
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Image { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Role { get; set; } = null!;
    public bool IsEmailVerified { get; set; }
    public string Message { get; set; } =
        "Account created successfully. A verification code has been sent to your email.";
}
