using Application.DTOs.TourCompany.Response;

namespace Application.DTOs.Admin.Response;

/// <summary>
/// Individual tour company statistics for the admin dashboard.
/// </summary>
public sealed record AdminCompanyDashboardResponse
{
    /// <summary>Detailed profile information of the company.</summary>
    public TourCompanyResponse Company { get; init; } = null!;

    /// <summary>Total number of bookings made for the company's tour packages.</summary>
    public int BookingsCount { get; init; }

    /// <summary>Total number of tour packages owned by the company.</summary>
    public int TotalTourPackages { get; init; }

    /// <summary>Average of the average rating of each rated tour package.</summary>
    public double AverageRating { get; init; }

    /// <summary>Sum of reviews across all of the company's tour packages.</summary>
    public int ReviewsCount { get; init; }

    /// <summary>Total revenue from completed bookings on completed tour packages.</summary>
    public decimal TotalRevenue { get; init; }

    /// <summary>Monthly bookings for the last 12 months (oldest to newest).</summary>
    public IReadOnlyList<MonthlyGrowth> BookingGrowth { get; init; } = [];

    /// <summary>Monthly created tour packages for the last 12 months (oldest to newest).</summary>
    public IReadOnlyList<MonthlyGrowth> TourPackageGrowth { get; init; } = [];
}
