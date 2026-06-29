using Application.DTOs.Companion.Request;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Validators.Companion
{
    public sealed class CompanionCreateValidator : AbstractValidator<CompanionCreateRequest>
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageBytes = 5 * 1024 * 1024;

        public CompanionCreateValidator()
        {
            RuleFor(x => x.Firstname)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters");

            RuleFor(x => x.Lastname)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .MaximumLength(25).WithMessage("Phone must not exceed 25 characters");

            RuleFor(x => x.NationalityCountryId)
                .GreaterThan(0).WithMessage("Nationality country is required");

            RuleFor(x => x.ResidentialCityId)
                .GreaterThan(0).WithMessage("Residential city is required");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required")
                .Must(g => g == "Male" || g == "Female")
                .WithMessage("Gender must be either 'Male' or 'Female'");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("Date of birth must be in the past");

            RuleFor(x => x.Relationship)
                .IsInEnum().WithMessage("Relationship is invalid");

            RuleFor(x => x.NationalIdCard)
                .Must(IsValidImage).WithMessage("National ID image must be JPG/PNG/WEBP and at most 5 MB")
                .When(x => x.NationalIdCard != null && x.NationalIdCard.Length > 0);

            RuleFor(x => x.PassportScan)
                .Must(IsValidImage).WithMessage("Passport image must be JPG/PNG/WEBP and at most 5 MB")
                .When(x => x.PassportScan != null && x.PassportScan.Length > 0);

            RuleFor(x => x.ResidencyCard)
                .Must(IsValidImage).WithMessage("Residency card image must be JPG/PNG/WEBP and at most 5 MB")
                .When(x => x.ResidencyCard != null && x.ResidencyCard.Length > 0);
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
