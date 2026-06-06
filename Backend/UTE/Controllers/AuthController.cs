using Application.DTOs.Auth.Request;
using Application.DTOs.Auth.Response;
using Application.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UTE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    [AllowAnonymous]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<RegisterResponse>> Register(
        [FromForm] RegisterRequest request,
        CancellationToken ct)
    {
        var response = await _auth.RegisterAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("verify-otp")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> VerifyOtp(
        [FromForm] VerifyOtpRequest request,
        CancellationToken ct)
    {
        var response = await _auth.VerifyOtpAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("resend-otp")]
    [AllowAnonymous]
    public async Task<ActionResult<OtpResponse>> ResendOtp(
        [FromForm] ResendOtpRequest request,
        CancellationToken ct)
    {
        var response = await _auth.ResendOtpAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromForm] LoginRequest request,
        CancellationToken ct)
    {
        var response = await _auth.LoginAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<ActionResult<OtpResponse>> ForgotPassword(
        [FromForm] ForgotPasswordRequest request,
        CancellationToken ct)
    {
        var response = await _auth.ForgotPasswordAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(
        [FromForm] ResetPasswordRequest request,
        CancellationToken ct)
    {
        await _auth.ResetPasswordAsync(request, ct);
        return Ok(new { message = "Password has been reset successfully. You can now log in with your new password." });
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        // JWT is stateless: the client is responsible for discarding the token.
        // This endpoint exists so clients have a single place to call when signing out.
        return Ok(new { message = "Logged out successfully. Please discard your token on the client." });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }
}
