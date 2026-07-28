using Application.Common.Constants;
using Application.Common.Logging;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Application.Interfaces.Auth;
using Application.DTOs.Auth.Request;
using Application.DTOs.Auth.Response;

namespace Application.Services;

public class AuthService : IAuthService
{
    private const int OtpLength = 6;
    private const int OtpExpiryMinutes = 10;
    private const int MaxOtpAttempts = 5;

    private const string PurposeEmailVerification = "EmailVerification";
    private const string PurposePasswordReset = "PasswordReset";

    private readonly IUserRepository _users;
    private readonly IGenericRepository<EmailVerification> _verifications;
    private readonly IGenericRepository<Role> _roles;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _tokens;
    private readonly IEmailSender _email;
    private readonly ILogger<AuthService> _logger;
    private const string ObjectName = "Account";

    public AuthService(
        IUserRepository users,
        IGenericRepository<EmailVerification> verifications,
        IGenericRepository<Role> roles,
        IPasswordHasher hasher,
        IJwtTokenGenerator tokens,
        IEmailSender email,
        ILogger<AuthService> logger)
    {
        _users = users;
        _verifications = verifications;
        _roles = roles;
        _hasher = hasher;
        _tokens = tokens;
        _email = email;
        _logger = logger;
    }

public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            _logger.StartOperation("Register", ObjectName, 0);

            var email = request.Email.Trim().ToLowerInvariant();

            if (await _users.EmailExistsAsync(email, ct))
                throw new ConflictException(ExceptionMessages.Conflict("Email", "already registered"));

            var role = await _roles.Query()
                .FirstOrDefaultAsync(r => r.RoleName == request.RoleName, ct)
                ?? throw new NotFoundException(ExceptionMessages.NotFound("Role", 0));

        var now = DateTime.UtcNow;
        var user = new User
        {
            Email              = email,
            Password           = _hasher.Hash(request.Password),
            RoleId             = role.RoleId,
            CreatedAtUtc       = now,
            UpdatedAtUtc       = now,
            IsEmailVerified    = false,
        };

        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        await IssueAndSendOtpAsync(user, PurposeEmailVerification, ct);

        return new RegisterResponse
        {
            UserId          = user.Id,
            Email           = user.Email,
            IsEmailVerified = false
        };
    }

public async Task<AuthResponse> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken ct = default)
        {
            _logger.StartOperation("Verify OTP", ObjectName, 0);

            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _users.GetByEmailAsync(email, ct)
                       ?? throw new NotFoundException(ExceptionMessages.NotFound(ObjectName, 0));

            if (user.IsEmailVerified)
                throw new ConflictException(ExceptionMessages.Conflict("Email", "already verified"));

            var verification = await _verifications.Query()
                .Where(v => v.UserId == user.Id && !v.IsUsed && v.Purpose == PurposeEmailVerification)
                .OrderByDescending(v => v.CreatedAtUtc)
                .FirstOrDefaultAsync(ct)
                ?? throw new NotFoundException("No active verification code. Please request a new one.");

            if (verification.ExpiresAt < DateTime.UtcNow)
                throw new AuthException(ExceptionMessages.AuthFailure("verification code expired"));

            if (verification.Attempts >= MaxOtpAttempts)
                throw new ForbiddenException(ExceptionMessages.Forbidden("verify", ObjectName));

            if (!FixedTimeEquals(verification.Code, request.Code))
            {
                verification.Attempts++;
                verification.UpdatedAtUtc = DateTime.UtcNow;
                _verifications.Update(verification);
                await _verifications.SaveChangesAsync(ct);
                throw new AuthException(
                    ExceptionMessages.AuthFailure($"invalid code: {MaxOtpAttempts - verification.Attempts} attempts left"));
            }

        verification.IsUsed = true;
        verification.UsedAt = DateTime.UtcNow;
        verification.UpdatedAtUtc = DateTime.UtcNow;
        _verifications.Update(verification);

        user.IsEmailVerified = true;
        user.UpdatedAtUtc = DateTime.UtcNow;
        _users.Update(user);

        await _users.SaveChangesAsync(ct);

        var (token, expiresAt) = _tokens.GenerateToken(user, 30);

        return new AuthResponse
        {
            UserId             = user.Id,
            Email              = user.Email,
            Role               = user.Role?.RoleName,
            IsEmailVerified    = true,
            IsProfileCompleted = user.PersonId.HasValue,
            Token              = token,
            ExpiresAt          = expiresAt
        };
    }

public async Task<OtpResponse> ResendOtpAsync(ResendOtpRequest request, CancellationToken ct = default)
        {
            _logger.StartOperation("Resend OTP", ObjectName, 0);

            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _users.GetByEmailAsync(email, ct)
                       ?? throw new NotFoundException(ExceptionMessages.NotFound(ObjectName, 0));

            if (user.IsEmailVerified)
                throw new ConflictException(ExceptionMessages.Conflict("Email", "already verified"));

        var expiresAt = await IssueAndSendOtpAsync(user, PurposeEmailVerification, ct);

        return new OtpResponse
        {
            Email        = user.Email,
            ExpiresAtUtc = expiresAt,
            Message      = "A new verification code has been sent to your email."
        };
    }

public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            _logger.StartOperation("Login", ObjectName, 0);

            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _users.GetByEmailAsync(email, ct);

            if (user is null || !_hasher.Verify(request.Password, user.Password))
                throw new AuthException(ExceptionMessages.AuthFailure("invalid email or password"));

            if (!user.IsEmailVerified)
                throw new ForbiddenException(ExceptionMessages.Forbidden("login", ObjectName));

        var expiry = user.PersonId is null ? 30 : (int?)null;
        var (token, expiresAt) = _tokens.GenerateToken(user, expiry);
        return new AuthResponse
        {
            UserId             = user.Id,
            FirstName          = user.Person?.FirstName,
            LastName           = user.Person?.LastName,
            Email              = user.Email,
            Image              = user.Person?.ProfileImage,
            DateOfBirth        = user.Person?.DateOfBirth,
            Role               = user.Role?.RoleName,
            IsEmailVerified    = true,
            IsProfileCompleted = user.PersonId.HasValue,
            Token              = token,
            ExpiresAt          = expiresAt
        };
    }

    public async Task<OtpResponse> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _users.GetByEmailAsync(email, ct);

        // Always return success-looking response to avoid revealing whether the email exists.
        // But if the account does exist, send the reset code.
        if (user is not null)
        {
            var _ = await IssueAndSendOtpAsync(user, PurposePasswordReset, ct);
        }
        else
        {
            _logger.LogInformation("Password reset requested for non-existent email {Email}", email);
        }

        return new OtpResponse
        {
            Email        = email,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes),
            Message      = "If an account exists for this email, a password reset code has been sent."
        };
    }

public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
        {
            _logger.StartOperation("Reset Password", ObjectName, 0);

            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _users.GetByEmailAsync(email, ct)
                       ?? throw new AuthException(ExceptionMessages.AuthFailure("invalid email or reset code"));

            var verification = await _verifications.Query()
                .Where(v => v.UserId == user.Id && !v.IsUsed && v.Purpose == PurposePasswordReset)
                .OrderByDescending(v => v.CreatedAtUtc)
                .FirstOrDefaultAsync(ct)
                ?? throw new AuthException(ExceptionMessages.AuthFailure("invalid email or reset code"));

            if (verification.ExpiresAt < DateTime.UtcNow)
                throw new AuthException(ExceptionMessages.AuthFailure("reset code expired"));

            if (verification.Attempts >= MaxOtpAttempts)
                throw new ForbiddenException(ExceptionMessages.Forbidden("reset password", ObjectName));

        if (!FixedTimeEquals(verification.Code, request.Code))
        {
            verification.Attempts++;
            verification.UpdatedAtUtc = DateTime.UtcNow;
            _verifications.Update(verification);
            await _verifications.SaveChangesAsync(ct);
            throw new AuthException(
                    ExceptionMessages.AuthFailure($"invalid code: {MaxOtpAttempts - verification.Attempts} attempts left"));
        }

        // Mark code as used and update password
        verification.IsUsed = true;
        verification.UsedAt = DateTime.UtcNow;
        verification.UpdatedAtUtc = DateTime.UtcNow;
        _verifications.Update(verification);

        user.Password = _hasher.Hash(request.NewPassword);
        user.UpdatedAtUtc = DateTime.UtcNow;
        _users.Update(user);

        await _users.SaveChangesAsync(ct);

        _logger.LogInformation("Password reset successful for user {UserId}", user.Id);
    }

    private async Task<DateTime> IssueAndSendOtpAsync(User user, string purpose, CancellationToken ct)
    {
        // Invalidate any previous unused codes for this user with the same purpose.
        var oldCodes = await _verifications.WhereAsync(
            v => v.UserId == user.Id && !v.IsUsed && v.Purpose == purpose, ct);
        foreach (var old in oldCodes)
        {
            old.IsUsed = true;
            old.UsedAt = DateTime.UtcNow;
            old.UpdatedAtUtc = DateTime.UtcNow;
            _verifications.Update(old);
        }

        var code = GenerateNumericCode(OtpLength);
        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(OtpExpiryMinutes);

        var verification = new EmailVerification
        {
            UserId       = user.Id,
            Code         = code,
            Purpose      = purpose,
            ExpiresAt    = expiresAt,
            Attempts     = 0,
            IsUsed       = false,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        await _verifications.AddAsync(verification, ct);
        await _verifications.SaveChangesAsync(ct);

        _logger.LogInformation("OTP for {Email} ({Purpose}) is {Code} (expires at {ExpiresAt:u})",
            user.Email, purpose, code, expiresAt);

        try
        {
            await SendOtpEmailAsync(user, code, purpose, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Failed to send OTP email to {Email}. The code is stored; user can request a new one.",
                user.Email);
        }

        return expiresAt;
    }

    private async Task SendOtpEmailAsync(User user, string code, string purpose, CancellationToken ct)
    {
        var displayName = !string.IsNullOrWhiteSpace(user.Person?.FirstName)
            ? $"{user.Person?.FirstName} {user.Person?.LastName}".Trim()
            : user.Email;
        var greetingName = !string.IsNullOrWhiteSpace(user.Person?.FirstName) ? user.Person?.FirstName! : "there";

        var subject = purpose == PurposePasswordReset
            ? "Your UTE Tourism password reset code"
            : "Your UTE Tourism verification code";

        var html = Infrastructure_EmailTemplate(greetingName, code, OtpExpiryMinutes);
        await _email.SendAsync(
            user.Email,
            displayName,
            subject,
            html,
            ct);
    }

    private static string Infrastructure_EmailTemplate(string firstName, string code, int minutes) => $@"
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='UTF-8' />
<meta name='viewport' content='width=device-width, initial-scale=1.0' />
<meta name='color-scheme' content='dark' />
<meta name='supported-color-schemes' content='dark' />
<title>Verification code</title>
</head>
<body style='margin:0;padding:0;background-color:#0f0f14;font-family:Segoe UI,Roboto,Helvetica,Arial,sans-serif;color:#e2e8f0;'>
<div style='display:none;max-height:0;overflow:hidden;font-size:1px;line-height:1px;color:#0f0f14;opacity:0;'>Your UTE Tourism verification code. Expires in {minutes} minutes.</div>
<table role='presentation' cellpadding='0' cellspacing='0' border='0' width='100%' bgcolor='#0f0f14' style='background-color:#0f0f14;padding:32px 16px;'>
<tr><td align='center'>
<table role='presentation' cellpadding='0' cellspacing='0' border='0' width='560' bgcolor='#1a1a22' style='max-width:560px;width:100%;background-color:#1a1a22;border-radius:18px;border:1px solid #2a2a35;overflow:hidden;'>
<tr><td align='center' style='padding:44px 32px 16px;'>
<table role='presentation' cellpadding='0' cellspacing='0' border='0'><tr>
<td align='center' bgcolor='#7c3aed' style='width:84px;height:84px;background-color:#7c3aed;background-image:linear-gradient(135deg,#7c3aed 0%,#a78bfa 100%);border-radius:20px;text-align:center;vertical-align:middle;'>
<div style='font-size:44px;line-height:84px;'>&#9992;&#65039;</div>
</td></tr></table>
</td></tr>
<tr><td align='center' style='padding:8px 32px 4px;'>
<h1 style='margin:0;color:#c4b5fd;font-size:34px;font-weight:700;letter-spacing:0.5px;'>Verification code</h1>
</td></tr>
<tr><td align='center' style='padding:6px 32px 20px;'>
<p style='margin:0;font-size:30px;line-height:1;'>&#128274;</p>
</td></tr>
<tr><td style='padding:0 32px;'>
<hr style='border:none;border-top:1px solid #4c1d95;margin:0;' />
</td></tr>
<tr><td align='center' style='padding:28px 32px 0;'>
<p style='margin:0;color:#e2e8f0;font-size:16px;line-height:1.6;'>Hi <strong style='color:#ffffff;'>{firstName}</strong>,</p>
</td></tr>
<tr><td align='center' style='padding:8px 32px 24px;'>
<p style='margin:0;color:#cbd5e1;font-size:15px;line-height:1.7;'>Copy the code below to verify your <strong style='color:#ffffff;'>UTE Tourism</strong> account.</p>
</td></tr>
<tr><td align='center' style='padding:0 32px 24px;'>
<table role='presentation' cellpadding='0' cellspacing='0' border='0' width='100%'><tr>
<td align='center' bgcolor='#0d2818' style='background-color:#0d2818;border:1px solid #166534;border-radius:14px;padding:26px 16px;'>
<p style='margin:0;color:#4ade80;font-size:42px;font-weight:700;letter-spacing:14px;font-family:Courier New,Courier,monospace;text-shadow:0 0 12px rgba(74,222,128,0.35);'>{code}</p>
</td></tr></table>
</td></tr>
<tr><td align='center' style='padding:0 32px 18px;'>
<p style='margin:0;color:#e2e8f0;font-size:15px;line-height:1.7;'>The code can only be used once and expires in <strong style='color:#ffffff;'>{minutes} minutes</strong>.</p>
</td></tr>
<tr><td align='center' style='padding:0 32px 28px;'>
<p style='margin:0;color:#f87171;font-size:14px;line-height:1.6;'>If you did not request this code, please ignore this email &#9888;&#65039;.</p>
</td></tr>
<tr><td style='padding:0 32px;'>
<hr style='border:none;border-top:1px solid #2a2a35;margin:0;' />
</td></tr>
<tr><td align='center' style='padding:22px 32px 32px;'>
<p style='margin:0;color:#64748b;font-size:12px;line-height:1.6;'>&#169; 2026 UTE Tourism &mdash; Ultimate Travel Experience. All rights reserved.</p>
</td></tr>
</table>
</td></tr>
</table>
</body>
</html>";

    private static string GenerateNumericCode(int length)
    {
        Span<byte> buffer = stackalloc byte[length];
        RandomNumberGenerator.Fill(buffer);
        var chars = new char[length];
        for (int i = 0; i < length; i++)
            chars[i] = (char)('0' + buffer[i] % 10);
        return new string(chars);
    }

    private static bool FixedTimeEquals(string a, string b)
    {
        if (a.Length != b.Length) return false;
        return CryptographicOperations.FixedTimeEquals(
            System.Text.Encoding.UTF8.GetBytes(a),
            System.Text.Encoding.UTF8.GetBytes(b));
    }
}
