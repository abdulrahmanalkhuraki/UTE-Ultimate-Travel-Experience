namespace Application.DTOs.Admin.Response;

public sealed record AdminTouristsDashboardResponse
{
    public int ActiveTourists { get; init; }
    public int DeletedTourists { get; init; }
    public int TotalTourists { get; init; }
    public IReadOnlyList<MonthlyGrowth> TouristGrowth { get; init; } = [];
}