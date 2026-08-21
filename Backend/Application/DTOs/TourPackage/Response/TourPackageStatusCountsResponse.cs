namespace Application.DTOs.TourPackage.Response;

public sealed record TourPackageStatusCountsResponse
{
    public int Pending { get; init; }
    public int Active { get; init; }
    public int Completed { get; init; }
    public int Cancelled { get; init; }
    public int Rejected { get; init; }
}
