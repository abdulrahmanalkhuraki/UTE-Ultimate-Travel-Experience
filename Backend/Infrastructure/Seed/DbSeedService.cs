using System.Reflection;
using System.Text.Json;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seed;

public class DbSeedService : IDbSeedService
{
    private readonly AppDbContext _context;

    public DbSeedService(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        await SeedAttractionCategoriesAsync(ct);
        await SeedCountriesAsync(ct);
        await SeedCitiesAsync(ct);
    }

    private async Task SeedAttractionCategoriesAsync(CancellationToken ct)
    {
        if (await _context.AttractionCategories.AnyAsync(ct))
            return;

        var items = await LoadJsonAsync<AttractionCategoryJson>("attraction_categories.json", ct);

        _context.AttractionCategories.AddRange(items.Select(x => new AttractionCategory
        {
            CategoryId = x.CategoryId,
            EnCategoryName = x.EnCategoryName,
            ArCategoryName = x.ArCategoryName
        }));

        await WithIdentityInsert("AttractionCategories", ct);
    }

    private async Task SeedCountriesAsync(CancellationToken ct)
    {
        if (await _context.Countries.AnyAsync(ct))
            return;

        var items = await LoadJsonAsync<CountryJson>("countries.json", ct);

        _context.Countries.AddRange(items.Select(x => new Country
        {
            Id = x.Id,
            EnCountryName = x.EnCountryName,
            ArCountryName = x.ArCountryName,
            CountryCode = x.CountryCode
        }));

        await WithIdentityInsert("Countries", ct);
    }

    private async Task SeedCitiesAsync(CancellationToken ct)
    {
        if (await _context.Cities.AnyAsync(ct))
            return;

        var items = await LoadJsonAsync<CityJson>("cities.json", ct);

        _context.Cities.AddRange(items.Select(x => new City
        {
            Id = x.Id,
            EnCityName = x.EnCityName,
            ArCityName = x.ArCityName,
            Image = x.Image,
            CountryId = x.CountryId
        }));

        await WithIdentityInsert("Cities", ct);
    }

    private async Task WithIdentityInsert(string tableName, CancellationToken ct)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(ct);

        await _context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{tableName}] ON", ct);

        try
        {
            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        finally
        {
            await _context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{tableName}] OFF", ct);
        }
    }

    private static async Task<List<T>> LoadJsonAsync<T>(string fileName, CancellationToken ct)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"Infrastructure.Data.SeedData.{fileName}";

        await using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");

        var items = await JsonSerializer.DeserializeAsync<List<T>>(stream, cancellationToken: ct)
            ?? throw new InvalidOperationException($"Failed to deserialize {fileName}.");

        return items;
    }

    private sealed record AttractionCategoryJson
    {
        public int CategoryId { get; init; }
        public string EnCategoryName { get; init; } = null!;
        public string ArCategoryName { get; init; } = null!;
    }

    private sealed record CountryJson
    {
        public int Id { get; init; }
        public string EnCountryName { get; init; } = null!;
        public string? ArCountryName { get; init; }
        public string CountryCode { get; init; } = null!;
    }

    private sealed record CityJson
    {
        public int Id { get; init; }
        public string EnCityName { get; init; } = null!;
        public string? ArCityName { get; init; }
        public string? Image { get; init; }
        public int CountryId { get; init; }
    }
}
