using Application.DTOs.Auth;

namespace Application.Interfaces.User;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<AuthResponse> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken ct = default);
    Task<OtpResponse> ResendOtpAsync(ResendOtpRequest request, CancellationToken ct = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
}
