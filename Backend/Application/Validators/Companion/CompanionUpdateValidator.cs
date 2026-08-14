using Application;
using Application.DTOs.Companion.Request;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Companion
{
    public sealed class CompanionUpdateValidator : AbstractValidator<CompanionUpdateRequest>
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageBytes = 5 * 1024 * 1024;

        private readonly IStringLocalizer<SharedResource> _localizer;

        public CompanionUpdateValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            When(x => x.Firstname != null, () =>
            {
                RuleFor(x => x.Firstname)
                    .NotEmpty().WithMessage(_localizer["First name cannot be empty"])
                    .MaximumLength(50).WithMessage(_localizer["First name must not exceed 50 characters"]);
            });

            When(x => x.Lastname != null, () =>
            {
                RuleFor(x => x.Lastname)
                    .NotEmpty().WithMessage(_localizer["Last name cannot be empty"])
                    .MaximumLength(50).WithMessage(_localizer["Last name must not exceed 50 characters"]);
            });

            When(x => x.Phone != null, () =>
            {
                RuleFor(x => x.Phone)
                    .NotEmpty().WithMessage(_localizer["Phone cannot be empty"])
                    .MaximumLength(25).WithMessage(_localizer["Phone must not exceed 25 characters"]);
            });

            RuleFor(x => x.NationalityCountryId)
                .GreaterThan(0).WithMessage(_localizer["Nationality country is required"])
                .When(x => x.NationalityCountryId.HasValue);

            RuleFor(x => x.ResidentialCityId)
                .GreaterThan(0).WithMessage(_localizer["Residential city is required"])
                .When(x => x.ResidentialCityId.HasValue);

            When(x => x.Gender != null, () =>
            {
                RuleFor(x => x.Gender)
                    .Must(g => g == "Male" || g == "Female")
                    .WithMessage(_localizer["Gender must be either 'Male' or 'Female'"]);
            });

            RuleFor(x => x.DateOfBirth)
                .Must(d => d!.Value < DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage(_localizer["Date of birth must be in the past"])
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.Relationship)
                .IsInEnum().WithMessage(_localizer["Relationship is invalid"])
                .When(x => x.Relationship.HasValue);

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
