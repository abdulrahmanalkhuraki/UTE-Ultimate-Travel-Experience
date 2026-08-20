using Application;
using Application.DTOs.TourCompany.Request;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.TourCompany
{
    public sealed class TourCompanyCreateValidator : AbstractValidator<TourCompanyCreateRequest>
    {
        // Allowed image content types for the logo and the license image.
        private static readonly string[] AllowedImageTypes =
            { "image/jpeg", "image/jpg", "image/png", "image/webp" };

        // 5 MB upload cap per image.
        private const long MaxImageBytes = 5 * 1024 * 1024;

        private readonly IStringLocalizer<SharedResource> _localizer;

        public TourCompanyCreateValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(_localizer["Company name is required"])
                .MinimumLength(3).WithMessage(_localizer["Company name must be at least 3 characters"])
                .MaximumLength(100).WithMessage(_localizer["Company name cannot exceed 100 characters"]);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(_localizer["Description is required"])
                .MaximumLength(500).WithMessage(_localizer["Description cannot exceed 500 characters"]);

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage(_localizer["Company location is required"])
                .MaximumLength(200).WithMessage(_localizer["Location cannot exceed 200 characters"]);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(_localizer["Phone number is required"])
                .Matches(@"^[\+]?[\d\s\-\(\)]{6,20}$").WithMessage(_localizer["Phone number format is invalid"])
                .MaximumLength(20).WithMessage(_localizer["Phone number cannot exceed 20 characters"]);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(_localizer["Email is required"])
                .EmailAddress().WithMessage(_localizer["Email format is invalid"])
                .MaximumLength(75).WithMessage(_localizer["Email cannot exceed 75 characters"]);

            RuleFor(x => x.FoundingDate)
                .NotNull().WithMessage(_localizer["Founding date is required"])
                .Must(d => !d.HasValue || d.Value <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage(_localizer["Founding date cannot be in the future"]);

            RuleFor(x => x.TourismLicenseNumber)
                .NotEmpty().WithMessage(_localizer["Tourism license number is required"])
                .MaximumLength(50).WithMessage(_localizer["Tourism license number cannot exceed 50 characters"]);

            RuleFor(x => x.BankAccount)
                .NotEmpty().WithMessage(_localizer["Bank account is required"])
                .MaximumLength(100).WithMessage(_localizer["Bank account cannot exceed 100 characters"]);

            RuleFor(x => x.About)
                .NotEmpty().WithMessage(_localizer["About is required"])
                .MaximumLength(2000).WithMessage(_localizer["About cannot exceed 2000 characters"]);

            RuleFor(x => x.Logo)
                .NotNull().WithMessage(_localizer["Logo is required"]);

            RuleFor(x => x.TourismLicenseImage)
                .NotNull().WithMessage(_localizer["Tourism license image is required"]);

            // Validate the uploaded files when present.
            RuleFor(x => x.Logo)
                .Must(BeAValidImage).WithMessage(_localizer["Logo must be a JPEG, PNG, or WebP image under 5 MB"])
                .When(x => x.Logo is not null);

            RuleFor(x => x.TourismLicenseImage)
                .Must(BeAValidImage).WithMessage(_localizer["Tourism license image must be a JPEG, PNG, or WebP image under 5 MB"])
                .When(x => x.TourismLicenseImage is not null);
        }

        private static bool BeAValidImage(Microsoft.AspNetCore.Http.IFormFile? file)
        {
            if (file is null) return false;
            if (file.Length <= 0 || file.Length > MaxImageBytes) return false;
            return AllowedImageTypes.Contains(file.ContentType?.ToLowerInvariant());
        }
    }
}
