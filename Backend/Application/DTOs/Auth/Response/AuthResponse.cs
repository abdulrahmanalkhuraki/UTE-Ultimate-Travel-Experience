namespace Application.DTOs.Auth.Response;

public class AuthResponse
{
    public int UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; } = null!;
    public string? Image { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Role { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsProfileCompleted { get; set; }
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
