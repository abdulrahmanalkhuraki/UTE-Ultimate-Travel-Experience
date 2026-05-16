using Application.Exceptions;
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IGenericRepository<Role> _roles;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _tokens;

    public AuthService(
        IUserRepository users,
        IGenericRepository<Role> roles,
        IPasswordHasher hasher,
        IJwtTokenGenerator tokens)
    {
        _users = users;
        _roles = roles;
        _hasher = hasher;
        _tokens = tokens;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var username = request.Username.Trim();

        var role = await _roles.FirstOrDefaultAsync(r => r.Id == request.Role, ct)
                   ?? throw new NotFoundException($"Role with id {request.Role} does not exist.");

        if (role.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            throw new ForbiddenException("Registering as Admin is not allowed from this endpoint.");

        if (await _users.EmailExistsAsync(email, ct))
            throw new ConflictException("This email is already registered.");

        if (await _users.UsernameExistsAsync(username, ct))
            throw new ConflictException("This username is already taken.");

        var now = DateTime.UtcNow;
        var user = new User
        {
            Username  = username,
            Email     = email,
            Password  = _hasher.Hash(request.Password),
            Phone     = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            Image     = string.IsNullOrWhiteSpace(request.Image) ? null : request.Image.Trim(),
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
            Status    = 1,
            RoleId    = request.Role
        };

        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        var fresh = await _users.GetByIdWithRoleAsync(user.Id, ct)
                    ?? throw new InvalidOperationException("Failed to load the created user.");

        return BuildResponse(fresh);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _users.GetByEmailAsync(email, ct);

        if (user is null || !_hasher.Verify(request.Password, user.Password))
            throw new AuthException("Invalid email or password.");

        if (user.Status != 1)
            throw new AuthException("This account is disabled. Please contact support.");

        return BuildResponse(user);
    }

    private AuthResponse BuildResponse(User user)
    {
        var (token, expiresAt) = _tokens.GenerateToken(user);
        return new AuthResponse
        {
            UserId    = user.Id,
            Username  = user.Username,
            Email     = user.Email,
            Role      = user.Role?.RoleName ?? string.Empty,
            Token     = token,
            ExpiresAt = expiresAt
        };
    }
}
