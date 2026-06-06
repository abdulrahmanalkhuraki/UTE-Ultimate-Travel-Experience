using Application.Common.Validation;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User.Request;

/// <summary>
/// Profile completion request for a Tour Company owner.
/// The role is fixed to "TourCompany" on the server and cannot be chosen by the client.
/// Passport and current-location fields are intentionally omitted for this flow.
/// </summary>
public class CompleteCompanyProfileRequest
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^[\+]?[\d\s\-\(\)]{6,20}$",
        ErrorMessage = "Phone number format is invalid.")]
    [StringLength(20)]
    public string Phone { get; set; } = null!;

    [Required(ErrorMessage = "Place of residence is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Place of residence must be between 2 and 100 characters.")]
    public string PlaceOfResidence { get; set; } = null!;

    [Required(ErrorMessage = "Gender is required.")]
    [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be either 'Male' or 'Female'.")]
    public string Gender { get; set; } = null!;

    [Required(ErrorMessage = "Date of birth is required.")]
    [DateOfBirth(MinAge = 18, MaxAge = 120)]
    [DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; }

    [Required(ErrorMessage = "National number is required.")]
    [StringLength(50, MinimumLength = 4, ErrorMessage = "National number must be between 4 and 50 characters.")]
    public string NationalNumber { get; set; } = null!;

    [Required(ErrorMessage = "Bank account is required.")]
    [StringLength(100, MinimumLength = 4, ErrorMessage = "Bank account must be between 4 and 100 characters.")]
    public string BankAccount { get; set; } = null!;

    public IFormFile? Image { get; set; }

    [Required(ErrorMessage = "National ID image is required.")]
    public IFormFile NationalIdImage { get; set; } = null!;
}
