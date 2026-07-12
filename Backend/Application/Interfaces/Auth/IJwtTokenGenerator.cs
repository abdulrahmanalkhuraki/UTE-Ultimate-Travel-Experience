using Domain.Entities;

namespace Application.Interfaces.Auth;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken(Domain.Entities.User user, int? expiresInMinutes = null);
}
