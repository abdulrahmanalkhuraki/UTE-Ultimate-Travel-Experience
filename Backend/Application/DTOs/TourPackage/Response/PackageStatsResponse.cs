namespace Application.DTOs.TourPackage.Response;

public sealed record PackageStatsResponse
{
    public int TotalPackages { get; init; }
    public int ActivePackages { get; init; }
    public int RejectedPackages { get; init; }
    public int CompletedPackages { get; init; }
    public int CancelledPackages { get; init; }

    public IReadOnlyList<MonthlyPackageCount> MonthlyPublished { get; init; } = [];
}

public sealed record MonthlyPackageCount
{
    public int Year { get; init; }
    public int Month { get; init; }
    public string MonthName { get; init; } = string.Empty;
    public int PublishedCount { get; init; }
}