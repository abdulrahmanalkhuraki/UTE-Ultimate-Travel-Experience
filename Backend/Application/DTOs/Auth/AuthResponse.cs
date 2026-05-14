namespace Application.DTOs.Auth;

public class AuthResponse
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Image { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Role { get; set; } = null!;
    public bool IsApproved { get; set; }
    public string? Token { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
