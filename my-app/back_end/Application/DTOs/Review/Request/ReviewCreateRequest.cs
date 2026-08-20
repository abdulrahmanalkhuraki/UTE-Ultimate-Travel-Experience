namespace Application.DTOs.Review.Request
{
    public sealed record ReviewCreateRequest
    (
        string comment,
        int PackageId
    );
}
