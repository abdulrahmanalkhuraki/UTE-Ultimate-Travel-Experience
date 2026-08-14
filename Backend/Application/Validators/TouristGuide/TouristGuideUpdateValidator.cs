using Application;
using Application.DTOs.TouristGuide.Request;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Application.Validators.TouristGuide
{
    public sealed class TouristGuideUpdateValidator : AbstractValidator<TouristGuideUpdateRequest>
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageBytes = 5 * 1024 * 1024;

        private readonly IStringLocalizer<SharedResource> _localizer;

        public TouristGuideUpdateValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(_localizer["First name cannot be empty"])
                .MaximumLength(100).WithMessage(_localizer["First name must not exceed 100 characters"])
                .When(x => x.FirstName != null);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(_localizer["Last name cannot be empty"])
                .MaximumLength(100).WithMessage(_localizer["Last name must not exceed 100 characters"])
                .When(x => x.LastName != null);

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage(_localizer["Phone cannot be empty"])
                .MaximumLength(25).WithMessage(_localizer["Phone must not exceed 25 characters"])
                .When(x => x.Phone != null);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(_localizer["Email cannot be empty"])
                .EmailAddress().WithMessage(_localizer["Email is not valid"])
                .MaximumLength(50).WithMessage(_localizer["Email must not exceed 50 characters"])
                .When(x => x.Email != null);

            RuleFor(x => x.NationalityCountryId)
                .GreaterThan(0).WithMessage(_localizer["Nationality (country) is required"])
                .When(x => x.NationalityCountryId.HasValue);

            RuleFor(x => x.ResidentialCityId)
                .GreaterThan(0).WithMessage(_localizer["Residential city is required"])
                .When(x => x.ResidentialCityId.HasValue);

            RuleFor(x => x.DateOfBirth)
                .Must(d => d!.Value < DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage(_localizer["Date of birth must be in the past"])
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.YearsOfExperiance)
                .InclusiveBetween(0, 70).WithMessage(_localizer["Years of experience must be between 0 and 70"])
                .When(x => x.YearsOfExperiance.HasValue);

            RuleFor(x => x.Bio)
                .NotEmpty().WithMessage(_localizer["Bio cannot be empty"])
                .MaximumLength(1000).WithMessage(_localizer["Bio must not exceed 1000 characters"])
                .When(x => x.Bio != null);

            RuleFor(x => x.NationalNumber)
                .NotEmpty().WithMessage(_localizer["National number cannot be empty"])
                .MaximumLength(50).WithMessage(_localizer["National number must not exceed 50 characters"])
                .When(x => x.NationalNumber != null);

            RuleFor(x => x.PassportNumber)
                .MaximumLength(50).WithMessage(_localizer["Passport number must not exceed 50 characters"])
                .When(x => x.PassportNumber != null);

            RuleFor(x => x.Languages)
                .MaximumLength(250).WithMessage(_localizer["Languages must not exceed 250 characters"])
                .When(x => x.Languages != null);

            RuleFor(x => x.ProfileImage)
                .Must(IsValidImage).WithMessage(_localizer["Profile image must be JPG/PNG/WEBP and at most 5 MB"])
                .When(x => x.ProfileImage != null);

            RuleFor(x => x.NationalIdImage)
                .Must(IsValidImage).WithMessage(_localizer["National ID image must be JPG/PNG/WEBP and at most 5 MB"])
                .When(x => x.NationalIdImage != null);

            RuleFor(x => x.PassportImage)
                .Must(IsValidImage).WithMessage(_localizer["Passport image must be JPG/PNG/WEBP and at most 5 MB"])
                .When(x => x.PassportImage != null);

            RuleFor(x => x.ResidencyCard)
                .Must(IsValidImage).WithMessage(_localizer["Residency card must be JPG/PNG/WEBP and at most 5 MB"])
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
