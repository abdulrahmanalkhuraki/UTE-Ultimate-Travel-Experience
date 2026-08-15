namespace Application.DTOs.Admin.Response;


public sealed record AdminDashboardResponse
{
    public int ActiveTourists { get; init; }
    public int ActiveCompanies { get; init; }
    public TourPackageCounts TourPackages { get; init; } = new();
    public decimal TotalRevenue { get; init; }
    public decimal CommissionRate { get; init; }
    public IReadOnlyList<MonthlyGrowth> TouristGrowth { get; init; } = [];
    public IReadOnlyList<MonthlyGrowth> TourPackageGrowth { get; init; } = [];
}

public sealed record TourPackageCounts
{
    public int Active { get; init; }
    public int Completed { get; init; }
}

public sealed record MonthlyGrowth
{
    public string Month { get; init; } = string.Empty;
    public int Count { get; init; }
}
