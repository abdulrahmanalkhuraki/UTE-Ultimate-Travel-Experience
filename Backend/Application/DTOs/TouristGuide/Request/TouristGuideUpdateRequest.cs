using Microsoft.AspNetCore.Http;

namespace Application.DTOs.TouristGuide.Request
{
    public sealed record TouristGuideUpdateRequest
    (
        string? FirstName,
        string? LastName,
        string? Phone,
        string? Email,
        int? NationalityCountryId,
        string? Gender,
        DateOnly? DateOfBirth,
        int? YearsOfExperiance,
        string? Bio,
        int? ResidentialCityId,
        string? NationalNumber,
        string? PassportNumber,
        string? Languages,
        bool? IsAvailable,
        IFormFile? ProfileImage,
        string? ProfileImageUrl,
        IFormFile? NationalIdImage,
        string? NationalIdCardUrl,
        IFormFile? PassportImage,
        string? PassportScanUrl
    );
}
