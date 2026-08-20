using Application.DTOs.TourPackage.Response;

namespace Application.DTOs.Admin.Response;

/// <summary>
/// Per-tour-package financial statistics for a company's completed packages (admin dashboard).
/// </summary>
public sealed record AdminTourPackageFinancialResponse
{
    /// <summary>Identifier of the tour package.</summary>
    public int TourPackageId { get; init; }

    /// <summary>Canonical (default-language) package name.</summary>
    public string PackageName { get; init; } = string.Empty;

    /// <summary>Url of the package image with the top display order.</summary>
    public string? PackageImage { get; init; }

    /// <summary>Distinct associated cities of the tour package.</summary>
    public List<PackageCityResponse> PackageCities { get; init; } = [];

    /// <summary>Number of completed bookings for this package.</summary>
    public int CompletedBookingsCount { get; init; }

    /// <summary>Average TotalCost across the package's completed bookings.</summary>
    public decimal AveragePrice { get; init; }

    /// <summary>Company earnings from this package after the platform commission: revenue × (1 − commissionRate).</summary>
    public decimal CompanyEarnings { get; init; }

    /// <summary>Platform profit from this package: revenue × commissionRate.</summary>
    public decimal OurProfit { get; init; }
}