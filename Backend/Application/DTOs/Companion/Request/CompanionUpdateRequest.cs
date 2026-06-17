using Domain.Enums;

namespace Application.DTOs.Companion.Request
{
    public sealed record CompanionUpdateRequest
    (
        string? Firstname,
        string? Lastname,
        string? Phone,
        int? NationalityCountryId,
        int? ResidentialCountryId,
        bool? Gender,
        DateOnly? DateOfBirth,
        string? IdCard,
        CompanionRelationship? Relationship
    );
}
