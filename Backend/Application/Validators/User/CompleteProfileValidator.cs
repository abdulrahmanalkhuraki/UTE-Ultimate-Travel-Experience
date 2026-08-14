using Application;
using Application.DTOs.User.Request;
using Microsoft.AspNetCore.Http;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.User
{
    public sealed class CompleteProfileValidator : AbstractValidator<CompleteProfileRequest>
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageBytes = 5 * 1024 * 1024; // 5 MB

        private readonly IStringLocalizer<SharedResource> _localizer;

        public CompleteProfileValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(_localizer["First name is required"])
                .Length(2, 50).WithMessage(_localizer["First name must be between 2 and 50 characters"]);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(_localizer["Last name is required"])
                .Length(2, 50).WithMessage(_localizer["Last name must be between 2 and 50 characters"]);

            RuleFor(x => x.ResidentialCityId)
                .GreaterThan(0).WithMessage(_localizer["Residential city is required"]);

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage(_localizer["Gender is required"])
                .Must(g => g == "Male" || g == "Female")
                .WithMessage(_localizer["Gender must be either 'Male' or 'Female'"]);

            RuleFor(x => x.DateOfBirth)
                .NotNull().WithMessage(_localizer["Date of birth is required"])
                .Must(d => BeAtLeast18YearsOld(d)).WithMessage(_localizer["User must be at least 18 years old"])
                .Must(d => BeReasonableAge(d)).WithMessage(_localizer["Date of birth is not reasonable (max age 120)"]);

            RuleFor(x => x.NationalNumber)
                .NotEmpty().WithMessage(_localizer["National number is required"])
                .Length(4, 50).WithMessage(_localizer["National number must be between 4 and 50 characters"]);

            RuleFor(x => x.PassportNumber)
                .NotEmpty().WithMessage(_localizer["Passport number is required"])
                .Length(4, 50).WithMessage(_localizer["Passport number must be between 4 and 50 characters"]);

            RuleFor(x => x.BankAccount)
                .NotEmpty().WithMessage(_localizer["Bank account is required"])
                .Length(4, 100).WithMessage(_localizer["Bank account must be between 4 and 100 characters"]);

            RuleFor(x => x.Phone)
                .Matches(@"^[\+]?[\d\s\-\(\)]{6,20}$").WithMessage(_localizer["Phone number format is invalid"])
                .When(x => !string.IsNullOrWhiteSpace(x.Phone));

            RuleFor(x => x.Image)
                .Must(IsValidImage).WithMessage(_localizer["Profile image must be JPG/PNG/WEBP and at most 5 MB"])
                .When(x => x.Image != null && x.Image.Length > 0);

            RuleFor(x => x.NationalIdImage)
                .NotNull().WithMessage(_localizer["National ID image is required"])
                .Must(f => f != null && f.Length > 0).WithMessage(_localizer["National ID image is required"])
                .Must(IsValidImage).WithMessage(_localizer["National ID image must be JPG/PNG/WEBP and at most 5 MB"]);

            RuleFor(x => x.PassportImage)
                .Must(IsValidImage).WithMessage(_localizer["Passport image must be JPG/PNG/WEBP and at most 5 MB"])
                .When(x => x.PassportImage != null && x.PassportImage.Length > 0);

            RuleFor(x => x.ResidencyCard)
                .Must(IsValidImage).WithMessage(_localizer["Passport image must be JPG/PNG/WEBP and at most 5 MB"])
                .When(x => x.ResidencyCard != null && x.ResidencyCard.Length > 0);
        }

        private static bool IsValidImage(IFormFile? file)
        {
            if (file == null || file.Length == 0) return false;
            if (file.Length > MaxImageBytes) return false;

            var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            return ext != null && AllowedImageExtensions.Contains(ext);
        }

        private static bool BeAtLeast18YearsOld(DateOnly date)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - date.Year;
            if (date > today.AddYears(-age)) age--;
            return age >= 18;
        }

        private static bool BeReasonableAge(DateOnly date)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - date.Year;
            return age <= 120;
        }
    }
}
