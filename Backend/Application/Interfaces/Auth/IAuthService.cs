using Application.DTOs.Auth.Request;
using Application.DTOs.Auth.Response;

<<<<<<<< HEAD:Backend/Application/Interfaces/Auth/IAuthService.cs
namespace Application.Interfaces.Auth;
========
namespace Application.Interfaces.User;
>>>>>>>> eb3c5c2000dac9b658f595448513569eb27a78bb:Backend/Application/Interfaces/User/IAuthService.cs

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<AuthResponse> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken ct = default);
    Task<OtpResponse> ResendOtpAsync(ResendOtpRequest request, CancellationToken ct = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<OtpResponse> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct = default);
    Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);
}
