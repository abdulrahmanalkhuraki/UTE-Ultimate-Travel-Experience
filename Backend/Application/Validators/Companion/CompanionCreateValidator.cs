using Application;
using Application.DTOs.Companion.Request;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Companion
{
    public sealed class CompanionCreateValidator : AbstractValidator<CompanionCreateRequest>
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageBytes = 5 * 1024 * 1024;

        private readonly IStringLocalizer<SharedResource> _localizer;

        public CompanionCreateValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Firstname)
                .NotEmpty().WithMessage(_localizer["First name is required"])
                .MaximumLength(50).WithMessage(_localizer["First name must not exceed 50 characters"]);

            RuleFor(x => x.Lastname)
                .NotEmpty().WithMessage(_localizer["Last name is required"])
                .MaximumLength(50).WithMessage(_localizer["Last name must not exceed 50 characters"]);

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage(_localizer["Phone is required"])
                .MaximumLength(25).WithMessage(_localizer["Phone must not exceed 25 characters"]);

            RuleFor(x => x.NationalityCountryId)
                .GreaterThan(0).WithMessage(_localizer["Nationality country is required"]);

            RuleFor(x => x.ResidentialCityId)
                .GreaterThan(0).WithMessage(_localizer["Residential city is required"]);

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage(_localizer["Gender is required"])
                .Must(g => g == "Male" || g == "Female")
                .WithMessage(_localizer["Gender must be either 'Male' or 'Female'"]);

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage(_localizer["Date of birth is required"])
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage(_localizer["Date of birth must be in the past"]);

            RuleFor(x => x.Relationship)
                .IsInEnum().WithMessage(_localizer["Relationship is invalid"]);

            RuleFor(x => x.NationalIdCard)
                .Must(IsValidImage).WithMessage(_localizer["National ID image must be JPG/PNG/WEBP and at most 5 MB"])
                .When(x => x.NationalIdCard != null && x.NationalIdCard.Length > 0);

            RuleFor(x => x.PassportScan)
                .Must(IsValidImage).WithMessage(_localizer["Passport image must be JPG/PNG/WEBP and at most 5 MB"])
                .When(x => x.PassportScan != null && x.PassportScan.Length > 0);

            RuleFor(x => x.ResidencyCard)
                .Must(IsValidImage).WithMessage(_localizer["Residency card image must be JPG/PNG/WEBP and at most 5 MB"])
                .When(x => x.ResidencyCard != null && x.ResidencyCard.Length > 0);

            RuleFor(x => x.ProfileImage)
                .Must(IsValidImage).WithMessage(_localizer["Profile image must be JPG/PNG/WEBP and at most 5 MB"])
                .When(x => x.ProfileImage != null && x.ProfileImage.Length > 0);
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
