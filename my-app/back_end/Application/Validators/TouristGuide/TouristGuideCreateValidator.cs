using Application;
using Application.DTOs.TouristGuide.Request;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Application.Validators.TouristGuide
{
    public sealed class TouristGuideCreateValidator : AbstractValidator<TouristGuideCreateRequest>
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageBytes = 5 * 1024 * 1024;

        private readonly IStringLocalizer<SharedResource> _localizer;

        public TouristGuideCreateValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(_localizer["First name is required"])
                .MaximumLength(100).WithMessage(_localizer["First name must not exceed 100 characters"]);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(_localizer["Last name is required"])
                .MaximumLength(100).WithMessage(_localizer["Last name must not exceed 100 characters"]);

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage(_localizer["Phone is required"])
                .MaximumLength(25).WithMessage(_localizer["Phone must not exceed 25 characters"]);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(_localizer["Email is required"])
                .EmailAddress().WithMessage(_localizer["Email is not valid"])
                .MaximumLength(50).WithMessage(_localizer["Email must not exceed 50 characters"]);

            RuleFor(x => x.NationalityCountryId)
                .GreaterThan(0).WithMessage(_localizer["Nationality (country) is required"]);

            RuleFor(x => x.ResidentialCityId)
                .GreaterThan(0).WithMessage(_localizer["Residential city is required"]);

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage(_localizer["Date of birth is required"])
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage(_localizer["Date of birth must be in the past"]);

            RuleFor(x => x.YearsOfExperiance)
                .InclusiveBetween(0, 70).WithMessage(_localizer["Years of experience must be between 0 and 70"]);

            RuleFor(x => x.Bio)
                .NotEmpty().WithMessage(_localizer["Bio is required"])
                .MaximumLength(1000).WithMessage(_localizer["Bio must not exceed 1000 characters"]);

            RuleFor(x => x.NationalNumber)
                .NotEmpty().WithMessage(_localizer["National number is required"])
                .MaximumLength(50).WithMessage(_localizer["National number must not exceed 50 characters"]);

            RuleFor(x => x.PassportNumber)
                .MaximumLength(50).WithMessage(_localizer["Passport number must not exceed 50 characters"]);

            RuleFor(x => x.Languages)
                .MaximumLength(250).WithMessage(_localizer["Languages must not exceed 250 characters"]);

            RuleFor(x => x.ProfileImage)
                .NotNull().WithMessage(_localizer["Profile image is required"])
                .Must(IsValidImage).WithMessage(_localizer["Profile image must be JPG/PNG/WEBP and at most 5 MB"])
                .When(x => x.ProfileImage != null);

            RuleFor(x => x.NationalIdCard)
                .NotNull().WithMessage(_localizer["National ID image is required"])
                .Must(IsValidImage).WithMessage(_localizer["National ID image must be JPG/PNG/WEBP and at most 5 MB"])
                .When(x => x.NationalIdCard != null);

            RuleFor(x => x.PassportScan)
                .NotNull().WithMessage(_localizer["Passport image is required"])
                .Must(IsValidImage).WithMessage(_localizer["Passport image must be JPG/PNG/WEBP and at most 5 MB"])
                .When(x => x.PassportScan != null);

            RuleFor(x => x.ResidencyCard)
                .NotNull().WithMessage(_localizer["Residency card is required"])
                .Must(IsValidImage).WithMessage(_localizer["Residency card image must be JPG/PNG/WEBP and at most 5 MB"])
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
