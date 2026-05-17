using Application.DTOs.Auth;
using Application.Interfaces;
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
        [FromBody] VerifyOtpRequest request,
        CancellationToken ct)
    {
        var response = await _auth.VerifyOtpAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("resend-otp")]
    [AllowAnonymous]
    public async Task<ActionResult<OtpResponse>> ResendOtp(
        [FromBody] ResendOtpRequest request,
        CancellationToken ct)
    {
        var response = await _auth.ResendOtpAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var response = await _auth.LoginAsync(request, ct);
        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }
}
