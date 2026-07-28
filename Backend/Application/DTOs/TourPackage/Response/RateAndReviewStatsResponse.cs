namespace Application.DTOs.TourPackage.Response;

public sealed record RateAndReviewStatsResponse
{
    public double AverageRating { get; init; }
    public int TotalRatings { get; init; }
    public int TotalReviews { get; init; }
    public IReadOnlyList<MonthlyRateReviewCount> MonthlyStats { get; init; } = [];
}

public sealed record MonthlyRateReviewCount
{
    public int Year { get; init; }
    public int Month { get; init; }
    public string MonthName { get; init; } = string.Empty;
    public int RatingCount { get; init; }
    public int ReviewCount { get; init; }
}