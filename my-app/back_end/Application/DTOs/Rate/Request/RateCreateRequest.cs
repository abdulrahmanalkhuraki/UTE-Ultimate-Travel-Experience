namespace Application.DTOs.Rate.Request
{
    public sealed record RateCreateRequest
        (
        int PackageId,
        int rateValue
        );
}
