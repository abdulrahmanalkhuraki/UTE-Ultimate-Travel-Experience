using System.Reflection;
using System.Text.Json;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

using Domain.Enums;

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
        await SeedPersonsAsync(ct);
        await SeedUsersAsync(ct);
        await SeedCompanionsAsync(ct);
        await SeedAttractionsAsync(ct);
        await SeedTourCompaniesAsync(ct);
        await SeedTouristGuidesAsync(ct);
        await SeedCompanyTouristGuidesAsync(ct);
        await SeedTourPackagesAsync(ct);
        await SeedItinerariesAsync(ct);
        await SeedActivitiesAsync(ct);
        await SeedPackageTouristGuidesAsync(ct);
        await SeedPackageAttractionsAsync(ct);
        await SeedPackageMediaAsync(ct);
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

    private async Task SeedAttractionsAsync(CancellationToken ct)
    {
        if (await _context.Attractions.AnyAsync(ct))
            return;

        var items = await LoadJsonAsync<AttractionJson>("attractions.json", ct);

        _context.Attractions.AddRange(items.Select(x => new Attraction
        {
            Id = x.Id,
            EnAttractionName = x.EnAttractionName,
            ArAttractionName = x.ArAttractionName,
            AttractionCategoryId = x.AttractionCategoryId,
            Description = x.Description,
            Longitude = x.Longitude,
            Latitude = x.Latitude,
            CityId = x.CityId
        }));

        await WithIdentityInsert("Attractions", ct);
    }

    private async Task SeedPersonsAsync(CancellationToken ct)
    {
        if (await _context.Persons.AnyAsync(ct))
            return;

        var items = await LoadJsonAsync<PersonJson>("persons.json", ct);

        _context.Persons.AddRange(items.Select(x => new Person
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            DateOfBirth = DateOnly.Parse(x.DateOfBirth),
            Gender = x.Gender,
            Phone = x.Phone,
            NationalNumber = x.NationalNumber,
            NationalityCountryId = x.NationalityCountryId,
            ResidentialCityId = x.ResidentialCityId
        }));

        await WithIdentityInsert("Persons", ct);
    }

    private async Task SeedUsersAsync(CancellationToken ct)
    {
        if (await _context.Users.AnyAsync(ct))
            return;

        var items = await LoadJsonAsync<UserJson>("users.json", ct);

        _context.Users.AddRange(items.Select(x => new User
        {
            Id = x.Id,
            Email = x.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(x.Password, workFactor: 11),
            RoleId = x.RoleId,
            PersonId = x.PersonId,
            IsEmailVerified = x.IsEmailVerified
        }));

        await WithIdentityInsert("Users", ct);
    }

    private async Task SeedCompanionsAsync(CancellationToken ct)
    {
        if (await _context.Companions.AnyAsync(ct))
            return;

        var items = await LoadJsonAsync<CompanionJson>("companions.json", ct);

        _context.Companions.AddRange(items.Select(x => new Companion
        {
            Id = x.Id,
            Relationship = (CompanionRelationship)x.Relationship,
            PersonId = x.PersonId,
            UserId = x.UserId
        }));

        await WithIdentityInsert("Companions", ct);
    }

    private async Task SeedTourCompaniesAsync(CancellationToken ct)
    {
        if (await _context.TourCompanies.AnyAsync(ct))
            return;

        var items = await LoadJsonAsync<TourCompanyJson>("tour_companies.json", ct);

        _context.TourCompanies.AddRange(items.Select(x => new TourCompany
        {
            Id = x.Id,
            Name = x.Name,
            Status = (Domain.Enums.TourCompanyStatus)x.Status,
            RejectionReason = x.RejectionReason,
            Description = x.Description,
            Logo = x.Logo,
            Location = x.Location,
            PhoneNumber = x.PhoneNumber,
            Email = x.Email,
            FoundingDate = x.FoundingDate != null ? DateOnly.Parse(x.FoundingDate) : null,
            TourismLicenseNumber = x.TourismLicenseNumber,
            TourismLicenseImage = x.TourismLicenseImage,
            BankAccount = x.BankAccount,
            About = x.About,
            UserId = x.UserId
        }));

        await WithIdentityInsert("TourCompanies", ct);
    }

    private async Task SeedTouristGuidesAsync(CancellationToken ct)
    {
        if (await _context.TouristGuides.AnyAsync(ct))
            return;

        var items = await LoadJsonAsync<TouristGuideJson>("tourist_guides.json", ct);

        foreach (var x in items)
        {
            var guide = new TouristGuide
            {
                Id = x.Id,
                Email = x.Email,
                YearsOfExperiance = x.YearsOfExperiance,
                Bio = x.Bio,
                Languages = x.Languages,
                LicenseScan = x.LicenseScan,
                IsAvailable = x.IsAvailable,
                PersonId = x.PersonId
            };
            _context.Entry(guide).Property("NatinalityCountryId").CurrentValue = x.NatinalityCountryId;
            _context.TouristGuides.Add(guide);
        }

        await WithIdentityInsert("TouristGuides", ct);
    }

    private async Task SeedCompanyTouristGuidesAsync(CancellationToken ct)
    {
        if (await _context.Company_TouristGuides.AnyAsync(ct))
            return;

        var items = await LoadJsonAsync<CompanyTouristGuideJson>("company_tourist_guides.json", ct);

        _context.Company_TouristGuides.AddRange(items.Select(x => new Company_TouristGuide
        {
            Id = x.Id,
            CompanyId = x.CompanyId,
            TouristGuideId = x.TouristGuideId
        }));

        await WithIdentityInsert("Company_TouristGuide", ct);
    }

    private async Task SeedTourPackagesAsync(CancellationToken ct)
    {
        if (await _context.TourPackages.AnyAsync(ct))
            return;

        var items = await LoadJsonAsync<TourPackageJson>("tour_packages.json", ct);

        _context.TourPackages.AddRange(items.Select(x => new TourPackage
        {
            Id = x.Id,
            PackageName = x.PackageName,
            Description = x.Description,
            MeetingPoint = x.MeetingPoint,
            PricePerPerson = x.PricePerPerson,
            Currency = x.Currency,
            DurationInDays = x.DurationInDays,
            AvailableSeats = x.AvailableSeats,
            CountryId = x.CountryId,
            StartDate = DateOnly.Parse(x.StartDate),
            EndDate = DateOnly.Parse(x.EndDate),
            RegistrationDeadline = DateOnly.Parse(x.RegistrationDeadline),
            ServiceLevel = (Domain.Enums.ServiceLevel)x.ServiceLevel,
            Status = (Domain.Enums.TourPackageStatus)x.Status,
            RejectionReason = x.RejectionReason,
            PublishCount = x.PublishCount,
            PublishedAtUtc = x.PublishedAtUtc != null ? DateTime.Parse(x.PublishedAtUtc) : null,
            IsDeleted = x.IsDeleted,
            CompanyId = x.CompanyId
        }));

        await WithIdentityInsert("TourPackages", ct);
    }

    private async Task SeedItinerariesAsync(CancellationToken ct)
    {
        if (await _context.Set<Itinerary>().AnyAsync(ct))
            return;

        var items = await LoadJsonAsync<ItineraryJson>("tour_package_itineraries.json", ct);

        _context.Set<Itinerary>().AddRange(items.Select(x => new Itinerary
        {
            Id = x.Id,
            DayNumber = x.DayNumber,
            DayTitle = x.DayTitle,
            DayDescription = x.DayDescription,
            PackageId = x.PackageId
        }));

        await WithIdentityInsert("Itineraries", ct);
    }

    private async Task SeedActivitiesAsync(CancellationToken ct)
    {
        if (await _context.Set<Activity>().AnyAsync(ct))
            return;

        var items = await LoadJsonAsync<ActivityJson>("tour_package_activities.json", ct);

        _context.Set<Activity>().AddRange(items.Select(x => new Activity
        {
            Id = x.Id,
            OrderNumber = x.OrderNumber,
            Title = x.Title,
            Description = x.Description,
            ImageUrl = x.ImageUrl,
            StartTime = TimeOnly.Parse(x.StartTime),
            EndTime = TimeOnly.Parse(x.EndTime),
            ItineraryId = x.ItineraryId
        }));

        await WithIdentityInsert("Activities", ct);
    }

    private async Task SeedPackageTouristGuidesAsync(CancellationToken ct)
    {
        if (await _context.TourPackage_TouristGuides.AnyAsync(ct))
            return;

        var items = await LoadJsonAsync<PackageTouristGuideJson>("tour_package_tourist_guides.json", ct);

        _context.TourPackage_TouristGuides.AddRange(items.Select(x => new TourPackage_TouristGuide
        {
            Id = x.Id,
            PackageId = x.PackageId,
            TouristGuideId = x.TouristGuideId
        }));

        await WithIdentityInsert("TourPackage_TouristGuide", ct);
    }

    private async Task SeedPackageAttractionsAsync(CancellationToken ct)
    {
        if (await _context.PackageCities.AnyAsync(ct))
            return;

        var items = await LoadJsonAsync<PackageAttractionJson>("tour_package_attractions.json", ct);

        _context.PackageCities.AddRange(items.Select(x => new TourPackage_Attraction
        {
            Id = x.Id,
            PackageId = x.PackageId,
            AttractionId = x.AttractionId
        }));

        await WithIdentityInsert("PackageAttractions", ct);
    }

    private async Task SeedPackageMediaAsync(CancellationToken ct)
    {
        if (await _context.TourPackageMedias.AnyAsync(ct))
            return;

        var items = await LoadJsonAsync<PackageMediaJson>("tour_package_media.json", ct);

        _context.TourPackageMedias.AddRange(items.Select(x => new TourPackageMedia
        {
            Id = x.Id,
            TourPackageId = x.TourPackageId,
            MediaUrl = x.MediaUrl,
            MediaType = (Domain.Enums.MediaType)x.MediaType,
            DisplayOrder = x.DisplayOrder
        }));

        await WithIdentityInsert("TourPackageMedias", ct);
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

    private sealed record AttractionJson
    {
        public int Id { get; init; }
        public string EnAttractionName { get; init; } = null!;
        public string ArAttractionName { get; init; } = null!;
        public int AttractionCategoryId { get; init; }
        public string? Description { get; init; }
        public decimal Longitude { get; init; }
        public decimal Latitude { get; init; }
        public int CityId { get; init; }
    }

    private sealed record PersonJson
    {
        public int Id { get; init; }
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string DateOfBirth { get; init; } = null!;
        public string Gender { get; init; } = null!;
        public string Phone { get; init; } = null!;
        public string? NationalNumber { get; init; }
        public int NationalityCountryId { get; init; }
        public int ResidentialCityId { get; init; }
    }

    private sealed record UserJson
    {
        public int Id { get; init; }
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
        public int RoleId { get; init; }
        public int PersonId { get; init; }
        public bool IsEmailVerified { get; init; }
    }

    private sealed record TourCompanyJson
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public int Status { get; init; }
        public string? RejectionReason { get; init; }
        public string? Description { get; init; }
        public string? Logo { get; init; }
        public string? Location { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Email { get; init; }
        public string? FoundingDate { get; init; }
        public string? TourismLicenseNumber { get; init; }
        public string? TourismLicenseImage { get; init; }
        public string? BankAccount { get; init; }
        public string? About { get; init; }
        public int UserId { get; init; }
    }

    private sealed record TouristGuideJson
    {
        public int Id { get; init; }
        public string Email { get; init; } = null!;
        public int YearsOfExperiance { get; init; }
        public string Bio { get; init; } = null!;
        public string? Languages { get; init; }
        public string? LicenseScan { get; init; }
        public bool IsAvailable { get; init; }
        public int PersonId { get; init; }
        public int NatinalityCountryId { get; init; }
    }

    private sealed record CompanyTouristGuideJson
    {
        public int Id { get; init; }
        public int CompanyId { get; init; }
        public int TouristGuideId { get; init; }
    }

    private sealed record TourPackageJson
    {
        public int Id { get; init; }
        public string PackageName { get; init; } = null!;
        public string? Description { get; init; }
        public string MeetingPoint { get; init; } = null!;
        public decimal PricePerPerson { get; init; }
        public string Currency { get; init; } = null!;
        public int DurationInDays { get; init; }
        public int AvailableSeats { get; init; }
        public int CountryId { get; init; }
        public string StartDate { get; init; } = null!;
        public string EndDate { get; init; } = null!;
        public string RegistrationDeadline { get; init; } = null!;
        public int ServiceLevel { get; init; }
        public int Status { get; init; }
        public string? RejectionReason { get; init; }
        public int PublishCount { get; init; }
        public string? PublishedAtUtc { get; init; }
        public bool IsDeleted { get; init; }
        public int CompanyId { get; init; }
    }

    private sealed record ItineraryJson
    {
        public int Id { get; init; }
        public int DayNumber { get; init; }
        public string DayTitle { get; init; } = null!;
        public string? DayDescription { get; init; }
        public int PackageId { get; init; }
    }

    private sealed record ActivityJson
    {
        public int Id { get; init; }
        public int OrderNumber { get; init; }
        public string Title { get; init; } = null!;
        public string? Description { get; init; }
        public string? ImageUrl { get; init; }
        public string StartTime { get; init; } = null!;
        public string EndTime { get; init; } = null!;
        public int ItineraryId { get; init; }
    }

    private sealed record PackageTouristGuideJson
    {
        public int Id { get; init; }
        public int PackageId { get; init; }
        public int TouristGuideId { get; init; }
    }

    private sealed record PackageAttractionJson
    {
        public int Id { get; init; }
        public int PackageId { get; init; }
        public int AttractionId { get; init; }
    }

    private sealed record PackageMediaJson
    {
        public int Id { get; init; }
        public int TourPackageId { get; init; }
        public string MediaUrl { get; init; } = null!;
        public int MediaType { get; init; }
        public int DisplayOrder { get; init; }
    }

    private sealed record CompanionJson
    {
        public int Id { get; init; }
        public int Relationship { get; init; }
        public int PersonId { get; init; }
        public int UserId { get; init; }
    }
}
