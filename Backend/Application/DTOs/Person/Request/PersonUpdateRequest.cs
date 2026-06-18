using Microsoft.AspNetCore.Http;


namespace Application.DTOs.Person.Request
{
    public sealed record PersonUpdateRequest
    (
        string? FirstName,
        string? LastName,
        IFormFile? ProfileImage,
        DateOnly? DateOfBirth,
        string? Gender,
        string? Phone,
        string? NationalNumber,
        IFormFile? NationalIdCard,
        string? PassportNumber,
        IFormFile? PassportScan,
        int? ResidentialCityId
    );
}
