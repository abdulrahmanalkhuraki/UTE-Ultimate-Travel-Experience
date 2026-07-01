using Application.DTOs.TouristGuide.Request;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Validators.TouristGuide
{
    public sealed class TouristGuideCreateValidator : AbstractValidator<TouristGuideCreateRequest>
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageBytes = 5 * 1024 * 1024;

        public TouristGuideCreateValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .MaximumLength(25).WithMessage("Phone must not exceed 25 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is not valid")
                .MaximumLength(50).WithMessage("Email must not exceed 50 characters");

            RuleFor(x => x.NationalityCountryId)
                .GreaterThan(0).WithMessage("Nationality (country) is required");

            RuleFor(x => x.ResidentialCityId)
                .GreaterThan(0).WithMessage("Residential city is required");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("Date of birth must be in the past");

            RuleFor(x => x.YearsOfExperiance)
                .InclusiveBetween(0, 70).WithMessage("Years of experience must be between 0 and 70");

            RuleFor(x => x.Bio)
                .NotEmpty().WithMessage("Bio is required")
                .MaximumLength(1000).WithMessage("Bio must not exceed 1000 characters");

            RuleFor(x => x.NationalNumber)
                .NotEmpty().WithMessage("National number is required")
                .MaximumLength(50).WithMessage("National number must not exceed 50 characters");

            RuleFor(x => x.PassportNumber)
                .MaximumLength(50).WithMessage("Passport number must not exceed 50 characters");

            RuleFor(x => x.Languages)
                .MaximumLength(250).WithMessage("Languages must not exceed 250 characters");

            RuleFor(x => x.ProfileImage)
                .NotNull().WithMessage("Profile image is required")
                .Must(IsValidImage).WithMessage("Profile image must be JPG/PNG/WEBP and at most 5 MB")
                .When(x => x.ProfileImage != null);

            RuleFor(x => x.NationalIdCard)
                .NotNull().WithMessage("National ID image is required")
                .Must(IsValidImage).WithMessage("National ID image must be JPG/PNG/WEBP and at most 5 MB")
                .When(x => x.NationalIdCard != null);

            RuleFor(x => x.PassportScan)
                .NotNull().WithMessage("Passport image is required")
                .Must(IsValidImage).WithMessage("Passport image must be JPG/PNG/WEBP and at most 5 MB")
                .When(x => x.PassportScan != null);

            RuleFor(x => x.ResidencyCard)
                .NotNull().WithMessage("Residency card is required")
                .Must(IsValidImage).WithMessage("Residency card must be JPG/PNG/WEBP and at most 5 MB")
                .When(x => x.ResidencyCard != null);
        }

        private static bool IsValidImage(IFormFile? file)
        {
            if (file == null || file.Length == 0) return false;
            if (file.Length > MaxImageBytes) return false;

            var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            return ext != null && AllowedImageExtensions.Contains(ext);
        }
    }
}
