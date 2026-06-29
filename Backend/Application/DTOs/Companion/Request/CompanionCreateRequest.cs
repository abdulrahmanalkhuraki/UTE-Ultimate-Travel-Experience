using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Companion.Request
{
    public sealed record CompanionCreateRequest
    (
        string Firstname,
        string Lastname,
        string Phone,
        int NationalityCountryId,
        int ResidentialCityId,
        string Gender,
        DateOnly DateOfBirth,
        string? NationalNumber,
        IFormFile? NationalIdCard,
        string? PassportNumber,
        IFormFile? PassportScan,
        IFormFile? ResidencyCard,
        CompanionRelationship Relationship
    );
}
