namespace Application.DTOs.Auth.Response;

public class OtpResponse
{
    public string Email { get; set; } = null!;
    public DateTime ExpiresAtUtc { get; set; }
    public string Message { get; set; } = null!;
}
