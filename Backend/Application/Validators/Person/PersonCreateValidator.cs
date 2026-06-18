using Application.DTOs.Person.Request;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;

namespace Application.Validators.Person
{
    public sealed class PersonCreateValidator : AbstractValidator<PersonCreateRequest>
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageBytes = 5 * 1024 * 1024; // 5 MB

        public PersonCreateValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .Length(2, 50).WithMessage("First name must be between 2 and 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .Length(2, 50).WithMessage("Last name must be between 2 and 50 characters");

            RuleFor(x => x.DateOfBirth)
                .NotNull().WithMessage("Date of birth is required")
                .Must(BeAtLeast18YearsOld).WithMessage("Person must be at least 18 years old")
                .Must(BeReasonableAge).WithMessage("Date of birth is not reasonable (max age 120)");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required")
                .Must(g => g == "Male" || g == "Female")
                .WithMessage("Gender must be either 'Male' or 'Female'");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .Matches(@"^[\+]?[\d\s\-\(\)]{6,20}$").WithMessage("Phone number format is invalid");

            RuleFor(x => x.ResidentialCityId)
                .GreaterThan(0).WithMessage("Residential city is required");

            RuleFor(x => x.NationalNumber)
                .Length(4, 50).WithMessage("National number must be between 4 and 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.NationalNumber));

            RuleFor(x => x.PassportNumber)
                .Length(4, 50).WithMessage("Passport number must be between 4 and 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.PassportNumber));

            // --- Image Validations ---

            RuleFor(x => x.ProfileImage)
                .Must(IsValidImage).WithMessage("Profile image must be JPG/PNG/WEBP and at most 5 MB")
                .When(x => x.ProfileImage != null && x.ProfileImage.Length > 0);

            RuleFor(x => x.NationalIdCard)
                .Must(IsValidImage).WithMessage("National ID card image must be JPG/PNG/WEBP and at most 5 MB")
                .When(x => x.NationalIdCard != null && x.NationalIdCard.Length > 0);

            RuleFor(x => x.PassportScan)
                .Must(IsValidImage).WithMessage("Passport scan image must be JPG/PNG/WEBP and at most 5 MB")
                .When(x => x.PassportScan != null && x.PassportScan.Length > 0);
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