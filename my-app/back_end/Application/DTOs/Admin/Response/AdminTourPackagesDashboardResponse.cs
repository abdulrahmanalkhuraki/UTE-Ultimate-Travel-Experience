namespace Application.DTOs.Admin.Response;

/// <summary>
/// Tour package statistics for the admin dashboard.
/// </summary>
public sealed record AdminTourPackagesDashboardResponse
{
    /// <summary>Total count of non-deleted tour packages regardless of status.</summary>
    public int TotalTourPackages { get; init; }

    /// <summary>Count of tour packages rejected by administrators.</summary>
    public int RejectedTourPackages { get; init; }

    /// <summary>Count of tour packages awaiting review/approval.</summary>
    public int PendingTourPackages { get; init; }

    /// <summary>Monthly created tour packages for the last 12 months (oldest to newest).</summary>
    public IReadOnlyList<MonthlyGrowth> TourPackageGrowth { get; init; } = [];
}