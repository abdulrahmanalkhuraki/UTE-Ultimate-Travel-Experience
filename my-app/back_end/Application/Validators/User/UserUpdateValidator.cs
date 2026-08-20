using Application;
using Application.DTOs.User.Request;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Application.Validators.User
{
    public sealed class UserUpdateValidator : AbstractValidator<UserUpdateRequest>
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageBytes = 5 * 1024 * 1024; // 5 MB

        private readonly IStringLocalizer<SharedResource> _localizer;

        public UserUpdateValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.FirstName)
                .MinimumLength(2).WithMessage(_localizer["First name must be at least 2 characters"])
                .MaximumLength(50).WithMessage(_localizer["First name must not exceed 50 characters"])
                .When(x => !string.IsNullOrWhiteSpace(x.FirstName));

            RuleFor(x => x.LastName)
                .MinimumLength(2).WithMessage(_localizer["Last name must be at least 2 characters"])
                .MaximumLength(50).WithMessage(_localizer["Last name must not exceed 50 characters"])
                .When(x => !string.IsNullOrWhiteSpace(x.LastName));

            RuleFor(x => x.Phone)
                .Matches(@"^[\+]?[\d\s\-\(\)]{6,20}$").WithMessage(_localizer["Phone number format is invalid"])
                .MaximumLength(20).WithMessage(_localizer["Phone must not exceed 20 characters"])
                .When(x => !string.IsNullOrWhiteSpace(x.Phone));

            RuleFor(x => x.DateOfBirth)
                .Must(d => BeAtLeast18YearsOld(d!.Value)).WithMessage(_localizer["User must be at least 18 years old"])
                .Must(d => BeReasonableAge(d!.Value)).WithMessage(_localizer["Date of birth is not reasonable (max age 120)"])
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.Gender)
                .Must(g => g == "Male" || g == "Female")
                .WithMessage(_localizer["Gender must be either 'Male' or 'Female'"])
                .When(x => !string.IsNullOrWhiteSpace(x.Gender));

            RuleFor(x => x.NationalNumber)
                .MinimumLength(4).WithMessage(_localizer["National number must be at least 4 characters"])
                .MaximumLength(50).WithMessage(_localizer["National number must not exceed 50 characters"])
                .When(x => !string.IsNullOrWhiteSpace(x.NationalNumber));

            RuleFor(x => x.PassportNumber)
                .MinimumLength(4).WithMessage(_localizer["Passport number must be at least 4 characters"])
                .MaximumLength(50).WithMessage(_localizer["Passport number must not exceed 50 characters"])
                .When(x => !string.IsNullOrWhiteSpace(x.PassportNumber));

            RuleFor(x => x.BankAccount)
                .MinimumLength(4).WithMessage(_localizer["Bank account must be at least 4 characters"])
                .MaximumLength(100).WithMessage(_localizer["Bank account must not exceed 100 characters"])
                .When(x => !string.IsNullOrWhiteSpace(x.BankAccount));

            RuleFor(x => x.Image)
                .Must(IsValidImage).WithMessage(_localizer["Profile image must be JPG/PNG/WEBP and at most 5 MB"])
                .When(x => x.Image != null && x.Image.Length > 0);

            RuleFor(x => x.NationalIdImage)
                .Must(IsValidImage).WithMessage(_localizer["National ID image must be JPG/PNG/WEBP and at most 5 MB"])
                .When(x => x.NationalIdImage != null && x.NationalIdImage.Length > 0);

            RuleFor(x => x.PassportImage)
                .Must(IsValidImage).WithMessage(_localizer["Passport image must be JPG/PNG/WEBP and at most 5 MB"])
                .When(x => x.PassportImage != null && x.PassportImage.Length > 0);

            // Reject empty payloads
            RuleFor(x => x)
                .Must(HasAtLeastOneField)
                .WithMessage(_localizer["At least one field must be provided to update"]);
        }

        private static bool HasAtLeastOneField(UserUpdateRequest x) =>
            !string.IsNullOrWhiteSpace(x.FirstName)
            || !string.IsNullOrWhiteSpace(x.LastName)
            || !string.IsNullOrWhiteSpace(x.Phone)
            || x.DateOfBirth.HasValue
            || !string.IsNullOrWhiteSpace(x.Gender)
            || !string.IsNullOrWhiteSpace(x.NationalNumber)
            || !string.IsNullOrWhiteSpace(x.PassportNumber)
            || !string.IsNullOrWhiteSpace(x.BankAccount)
            || (x.Image != null && x.Image.Length > 0)
            || (x.NationalIdImage != null && x.NationalIdImage.Length > 0)
            || (x.PassportImage != null && x.PassportImage.Length > 0);

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
