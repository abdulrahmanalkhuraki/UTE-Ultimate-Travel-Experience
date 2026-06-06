namespace Application.DTOs.Auth.Response;

public class RegisterResponse
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public bool IsEmailVerified { get; set; }
    public string Message { get; set; } =
        "Account created successfully. A verification code has been sent to your email.";
}
