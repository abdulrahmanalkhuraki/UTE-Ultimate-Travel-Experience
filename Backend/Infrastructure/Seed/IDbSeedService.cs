namespace Infrastructure.Seed;

public interface IDbSeedService
{
    Task SeedAsync(CancellationToken ct = default);
}
