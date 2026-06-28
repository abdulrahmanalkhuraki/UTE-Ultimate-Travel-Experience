using Microsoft.AspNetCore.Http;

namespace Application.DTOs.TouristGuide.Request
{
    public sealed record TouristGuideCreateRequest
    (
        string FirstName,
        string LastName,
        string Phone,
        string Email,
        int NationalityCountryId,
        string Gender,
        DateOnly DateOfBirth,
        int YearsOfExperiance,
        string Bio,
        int ResidentialCityId,
        string NationalNumber,
        string? PassportNumber,
        string? Languages,
        IFormFile? ProfileImage,
        IFormFile? NationalIdCard,
        IFormFile? PassportScan,
        IFormFile? ResidencyCard
    );
}
