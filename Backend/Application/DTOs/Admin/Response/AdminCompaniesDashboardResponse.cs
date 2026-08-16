namespace Application.DTOs.Admin.Response;

/// <summary>
/// Tour company statistics for the admin dashboard.
/// </summary>
public sealed record AdminCompaniesDashboardResponse
{
    /// <summary>Count of companies that are approved and whose owner is not deleted.</summary>
    public int ActiveCompanies { get; init; }

    /// <summary>Count of companies whose owner account has been soft-deleted.</summary>
    public int DeletedCompanies { get; init; }

    /// <summary>Count of companies awaiting administrative approval.</summary>
    public int PendingCompanies { get; init; }

    /// <summary>Monthly created companies for the last 12 months (oldest to newest).</summary>
    public IReadOnlyList<MonthlyGrowth> CompanyGrowth { get; init; } = [];
}
