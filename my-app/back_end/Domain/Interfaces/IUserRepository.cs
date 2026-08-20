using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

    Task<User?> GetByIdWithRoleAsync(int userId, CancellationToken ct = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
}
