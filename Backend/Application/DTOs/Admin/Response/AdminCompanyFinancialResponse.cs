namespace Application.DTOs.Admin.Response;

/// <summary>
/// Per-company financial statistics for the admin dashboard.
/// </summary>
public sealed record AdminCompanyFinancialResponse
{
    /// <summary>Identifier of the tour company.</summary>
    public int CompanyId { get; init; }

    /// <summary>Name of the tour company.</summary>
    public string CompanyName { get; init; } = string.Empty;

    /// <summary>Location of the tour company.</summary>
    public string? CompanyLocation { get; init; }

    /// <summary>Logo URL of the tour company.</summary>
    public string? CompanyLogo { get; init; }

    /// <summary>Company earnings after the platform commission: revenue × (1 − commission rate).</summary>
    public decimal CompanyEarnings { get; init; }

    /// <summary>Platform profit: company revenue multiplied by the commission rate.</summary>
    public decimal OurProfit { get; init; }
}