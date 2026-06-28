using Microsoft.AspNetCore.Http;

namespace Application.DTOs.User.Request;

public class CompleteProfileRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int ResidentialCityId { get; set; }
    public int NationalityCountryId { get; set; }
    public string Gender { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string NationalNumber { get; set; } = null!;
    public string PassportNumber { get; set; } = null!;
    public string BankAccount { get; set; } = null!;
    public string? Phone { get; set; }
    public IFormFile? Image { get; set; }
    public IFormFile NationalIdImage { get; set; } = null!;
    public IFormFile? PassportImage { get; set; } = null!;
    public IFormFile? ResidencyCard { get; set; } = null!;
}
