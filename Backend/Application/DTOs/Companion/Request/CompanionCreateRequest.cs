using Domain.Enums;

namespace Application.DTOs.Companion.Request
{
    public sealed record CompanionCreateRequest
    (
        string Firstname,
        string Lastname,
        string Phone,
        int NationalityCountryId,
        int ResidentialCountryId,
        bool Gender,
        DateOnly DateOfBirth,
        string? NationalNumber,
        string? NationalIdCard,
        string? PassportNumber,
        string? PassportScan,
        string? ResidencyCard,
        CompanionRelationship Relationship
    );
}
