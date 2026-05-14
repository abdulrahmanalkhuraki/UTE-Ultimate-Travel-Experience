using Application.Common;
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
    private readonly IFileStorage _files;

    public AuthService(
        IUserRepository users,
        IGenericRepository<Role> roles,
        IPasswordHasher hasher,
        IJwtTokenGenerator tokens,
        IFileStorage files)
    {
        _users = users;
        _roles = roles;
        _hasher = hasher;
        _tokens = tokens;
        _files = files;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();

        var role = await _roles.FirstOrDefaultAsync(r => r.RoleId == request.Role, ct)
                   ?? throw new NotFoundException($"Role with id {request.Role} does not exist.");

        if (role.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            throw new ForbiddenException("Registering as Admin is not allowed from this endpoint.");

        if (await _users.EmailExistsAsync(email, ct))
            throw new ConflictException("This email is already registered.");

        string? imageUrl = null;
        if (request.Image is { Length: > 0 })
            imageUrl = await _files.SaveAsync(request.Image, "profiles", ct);

        var now = DateTime.UtcNow;
        var user = new User
        {
            FirstName    = firstName,
            LastName     = lastName,
            Email        = email,
            Password     = _hasher.Hash(request.Password),
            Phone        = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            Image        = imageUrl,
            DateOfBirth  = request.DateOfBirth!.Value,
            CreatedAt    = now,
            UpdatedAt    = now,
            IsApproved   = false,
            RoleId       = request.Role
        };

        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        var fresh = await _users.GetByIdWithRoleAsync(user.UserId, ct)
                    ?? throw new InvalidOperationException("Failed to load the created user.");

        return BuildResponse(fresh, issueToken: false);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _users.GetByEmailAsync(email, ct);

        if (user is null || !_hasher.Verify(request.Password, user.Password))
            throw new AuthException("Invalid email or password.");

        if (!user.IsApproved)
            throw new ForbiddenException(
                "Your account is pending approval. Please wait for an administrator to approve your account.");

        return BuildResponse(user, issueToken: true);
    }

    private AuthResponse BuildResponse(User user, bool issueToken)
    {
        string? token = null;
        DateTime? expiresAt = null;

        if (issueToken)
        {
            var t = _tokens.GenerateToken(user);
            token = t.Token;
            expiresAt = t.ExpiresAt;
        }

        return new AuthResponse
        {
            UserId      = user.UserId,
            FirstName   = user.FirstName,
            LastName    = user.LastName,
            Email       = user.Email,
            Image       = user.Image,
            DateOfBirth = user.DateOfBirth,
            Role        = user.Role?.RoleName ?? string.Empty,
            IsApproved  = user.IsApproved,
            Token       = token,
            ExpiresAt   = expiresAt
        };
    }
}
