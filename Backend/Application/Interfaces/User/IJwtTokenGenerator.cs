using Domain.Entities;
namespace Application.Interfaces.User;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken(Domain.Entities.User user);
}
