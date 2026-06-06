using Application.DTOs.User.Request;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;

namespace Application.Validators.User
{
    public sealed class UpdateMeValidator : AbstractValidator<UpdateMeRequest>
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageBytes = 5 * 1024 * 1024; // 5 MB

        public UpdateMeValidator()
        {
            RuleFor(x => x.FirstName)
                .MinimumLength(2).WithMessage("First name must be at least 2 characters")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.FirstName));

            RuleFor(x => x.LastName)
                .MinimumLength(2).WithMessage("Last name must be at least 2 characters")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.LastName));

            RuleFor(x => x.Phone)
                .Matches(@"^[\+]?[\d\s\-\(\)]{6,20}$").WithMessage("Phone number format is invalid")
                .MaximumLength(20).WithMessage("Phone must not exceed 20 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone));

            RuleFor(x => x.DateOfBirth)
                .Must(d => BeAtLeast18YearsOld(d!.Value)).WithMessage("User must be at least 18 years old")
                .Must(d => BeReasonableAge(d!.Value)).WithMessage("Date of birth is not reasonable (max age 120)")
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.Gender)
                .Must(g => g == "Male" || g == "Female")
                .WithMessage("Gender must be either 'Male' or 'Female'")
                .When(x => !string.IsNullOrWhiteSpace(x.Gender));

            RuleFor(x => x.PlaceOfResidence)
                .MinimumLength(2).WithMessage("Place of residence must be at least 2 characters")
                .MaximumLength(100).WithMessage("Place of residence must not exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.PlaceOfResidence));

            RuleFor(x => x.CurrentLocation)
                .MinimumLength(2).WithMessage("Current location must be at least 2 characters")
                .MaximumLength(100).WithMessage("Current location must not exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.CurrentLocation));

            RuleFor(x => x.NationalNumber)
                .MinimumLength(4).WithMessage("National number must be at least 4 characters")
                .MaximumLength(50).WithMessage("National number must not exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.NationalNumber));

            RuleFor(x => x.PassportNumber)
                .MinimumLength(4).WithMessage("Passport number must be at least 4 characters")
                .MaximumLength(50).WithMessage("Passport number must not exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.PassportNumber));

            RuleFor(x => x.BankAccount)
                .MinimumLength(4).WithMessage("Bank account must be at least 4 characters")
                .MaximumLength(100).WithMessage("Bank account must not exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.BankAccount));

            RuleFor(x => x.Image)
                .Must(IsValidImage).WithMessage("Profile image must be JPG/PNG/WEBP and at most 5 MB")
                .When(x => x.Image != null && x.Image.Length > 0);

            RuleFor(x => x.NationalIdImage)
                .Must(IsValidImage).WithMessage("National ID image must be JPG/PNG/WEBP and at most 5 MB")
                .When(x => x.NationalIdImage != null && x.NationalIdImage.Length > 0);

            RuleFor(x => x.PassportImage)
                .Must(IsValidImage).WithMessage("Passport image must be JPG/PNG/WEBP and at most 5 MB")
                .When(x => x.PassportImage != null && x.PassportImage.Length > 0);

            // Password change rules
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required to change the password")
                .When(x => !string.IsNullOrWhiteSpace(x.NewPassword));

            RuleFor(x => x.NewPassword)
                .MinimumLength(8).WithMessage("New password must be at least 8 characters")
                .MaximumLength(100).WithMessage("New password must not exceed 100 characters")
                .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$")
                .WithMessage("New password must contain at least one uppercase letter, one lowercase letter, and one digit")
                .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from the current password")
                .When(x => !string.IsNullOrWhiteSpace(x.NewPassword));

            // Reject empty payloads
            RuleFor(x => x)
                .Must(HasAtLeastOneField)
                .WithMessage("At least one field must be provided to update");
        }

        private static bool HasAtLeastOneField(UpdateMeRequest x) =>
            !string.IsNullOrWhiteSpace(x.FirstName)
            || !string.IsNullOrWhiteSpace(x.LastName)
            || !string.IsNullOrWhiteSpace(x.Phone)
            || x.DateOfBirth.HasValue
            || !string.IsNullOrWhiteSpace(x.Gender)
            || !string.IsNullOrWhiteSpace(x.PlaceOfResidence)
            || !string.IsNullOrWhiteSpace(x.CurrentLocation)
            || !string.IsNullOrWhiteSpace(x.NationalNumber)
            || !string.IsNullOrWhiteSpace(x.PassportNumber)
            || !string.IsNullOrWhiteSpace(x.BankAccount)
            || (x.Image != null && x.Image.Length > 0)
            || (x.NationalIdImage != null && x.NationalIdImage.Length > 0)
            || (x.PassportImage != null && x.PassportImage.Length > 0)
            || !string.IsNullOrWhiteSpace(x.NewPassword);

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
