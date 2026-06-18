using Microsoft.AspNetCore.Http;

namespace Application.DTOs.User.Request
{
    public sealed record class UserUpdateRequest
    (
        string? FirstName,
        string? LastName,
        string? Phone,
        DateOnly? DateOfBirth,
        string? Gender,
        int? ResidentialCityId,
        string? NationalNumber,
        string? PassportNumber,
        string? BankAccount,
        IFormFile? Image,
        IFormFile? NationalIdImage,
        IFormFile? PassportImage
    );
}
