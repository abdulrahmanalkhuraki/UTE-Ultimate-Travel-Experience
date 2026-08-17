namespace Application.DTOs.Admin.Response;

/// <summary>
/// Financial (profit) statistics for the admin dashboard.
/// </summary>
public sealed record AdminFinancialDashboardResponse
{
    /// <summary>Total profit from completed bookings on completed, non-deleted tour packages.</summary>
    public decimal TotalProfit { get; init; }

    /// <summary>Platform commission rate applied to booking revenue.</summary>
    public decimal CommissionRate { get; init; }

    /// <summary>Monthly profit for the last 12 months grouped by tour package end date (oldest to newest).</summary>
    public IReadOnlyList<MonthlyProfit> ProfitGrowth { get; init; } = [];
}

public sealed record MonthlyProfit
{
    public string Month { get; init; } = string.Empty;

    public decimal Profit { get; init; }
}