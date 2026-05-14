using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext db) : base(db) { }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        Db.Users
          .Include(u => u.Role)
          .FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<User?> GetByIdWithRoleAsync(int userId, CancellationToken ct = default) =>
        Db.Users
          .Include(u => u.Role)
          .FirstOrDefaultAsync(u => u.UserId == userId, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        Db.Users.AnyAsync(u => u.Email == email, ct);
}
