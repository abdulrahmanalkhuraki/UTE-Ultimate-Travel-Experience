using Application.DTOs.User.Request;
using FluentValidation;
using System;
using System.Linq;

namespace Application.Validators.User
{
    public sealed class CompleteCompanyProfileValidator : AbstractValidator<CompleteCompanyProfileRequest>
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageBytes = 5 * 1024 * 1024; // 5 MB

        public CompleteCompanyProfileValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .Length(2, 50).WithMessage("First name must be between 2 and 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .Length(2, 50).WithMessage("Last name must be between 2 and 50 characters");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^[\+]?[\d\s\-\(\)]{6,20}$").WithMessage("Phone number format is invalid");

            RuleFor(x => x.PlaceOfResidence)
                .NotEmpty().WithMessage("Place of residence is required")
                .Length(2, 100).WithMessage("Place of residence must be between 2 and 100 characters");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required")
                .Must(g => g == "Male" || g == "Female")
                .WithMessage("Gender must be either 'Male' or 'Female'");

            RuleFor(x => x.DateOfBirth)
                .NotNull().WithMessage("Date of birth is required")
                .Must(d => BeAtLeast18YearsOld(d!.Value)).WithMessage("User must be at least 18 years old")
                .Must(d => BeReasonableAge(d!.Value)).WithMessage("Date of birth is not reasonable (max age 120)");

            RuleFor(x => x.NationalNumber)
                .NotEmpty().WithMessage("National number is required")
                .Length(4, 50).WithMessage("National number must be between 4 and 50 characters");

            RuleFor(x => x.BankAccount)
                .NotEmpty().WithMessage("Bank account is required")
                .Length(4, 100).WithMessage("Bank account must be between 4 and 100 characters");

            RuleFor(x => x.Image)
                .Must(IsValidImage).WithMessage("Profile image must be JPG/PNG/WEBP and at most 5 MB")
                .When(x => x.Image != null && x.Image.Length > 0);

            RuleFor(x => x.NationalIdImage)
                .NotNull().WithMessage("National ID image is required")
                .Must(f => f != null && f.Length > 0).WithMessage("National ID image is required")
                .Must(IsValidImage).WithMessage("National ID image must be JPG/PNG/WEBP and at most 5 MB");
        }

        private static bool IsValidImage(Microsoft.AspNetCore.Http.IFormFile? file)
        {
            if (file == null || file.Length == 0) return false;
            if (file.Length > MaxImageBytes) return false;

            var ext = System.IO.Path.GetExtension(file.FileName)?.ToLowerInvariant();
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
