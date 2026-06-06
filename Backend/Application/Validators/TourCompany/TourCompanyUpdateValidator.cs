using Application.DTOs.TourCompany.Request;
using FluentValidation;

namespace Application.Validators.TourCompany
{
    public sealed class TourCompanyUpdateValidator : AbstractValidator<TourCompanyUpdateRequest>
    {
        private static readonly string[] AllowedImageTypes =
            { "image/jpeg", "image/jpg", "image/png", "image/webp" };

        private const long MaxImageBytes = 5 * 1024 * 1024;

        public TourCompanyUpdateValidator()
        {
            // Partial update: every rule only fires when the field is actually provided.
            RuleFor(x => x.Name)
                .MinimumLength(3).WithMessage("Company name must be at least 3 characters")
                .MaximumLength(100).WithMessage("Company name cannot exceed 100 characters")
                .When(x => x.Name is not null);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description cannot be empty")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
                .When(x => x.Description is not null);

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location cannot be empty")
                .MaximumLength(200).WithMessage("Location cannot exceed 200 characters")
                .When(x => x.Location is not null);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number cannot be empty")
                .Matches(@"^[\+]?[\d\s\-\(\)]{6,20}$").WithMessage("Phone number format is invalid")
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
                .When(x => x.PhoneNumber is not null);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email cannot be empty")
                .EmailAddress().WithMessage("Email format is invalid")
                .MaximumLength(75).WithMessage("Email cannot exceed 75 characters")
                .When(x => x.Email is not null);

            RuleFor(x => x.FoundingDate)
                .Must(d => !d.HasValue || d.Value <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Founding date cannot be in the future")
                .When(x => x.FoundingDate is not null);

            RuleFor(x => x.TourismLicenseNumber)
                .NotEmpty().WithMessage("Tourism license number cannot be empty")
                .MaximumLength(50).WithMessage("Tourism license number cannot exceed 50 characters")
                .When(x => x.TourismLicenseNumber is not null);

            RuleFor(x => x.BankAccount)
                .NotEmpty().WithMessage("Bank account cannot be empty")
                .MaximumLength(100).WithMessage("Bank account cannot exceed 100 characters")
                .When(x => x.BankAccount is not null);

            RuleFor(x => x.About)
                .NotEmpty().WithMessage("About cannot be empty")
                .MaximumLength(2000).WithMessage("About cannot exceed 2000 characters")
                .When(x => x.About is not null);

            RuleFor(x => x.Logo)
                .Must(BeAValidImage).WithMessage("Logo must be a JPEG, PNG, or WebP image under 5 MB")
                .When(x => x.Logo is not null);

            RuleFor(x => x.TourismLicenseImage)
                .Must(BeAValidImage).WithMessage("Tourism license image must be a JPEG, PNG, or WebP image under 5 MB")
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
