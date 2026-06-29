namespace Application.DTOs.User.Request
{
    public sealed record UpdateLocationRequest
    (
        decimal Longitude,
        decimal Latitude
    );
}
