using Application.DTOs.TouristGuide.Request;
using FluentValidation;

namespace Application.Validators.TouristGuide
{
    public sealed class TouristGuideUpdateValidator : AbstractValidator<TouristGuideUpdateRequest>
    {
        public TouristGuideUpdateValidator()
        {
            When(x => x.FirstName != null, () =>
            {
                RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage("First name cannot be empty")
                    .MaximumLength(100).WithMessage("First name must not exceed 100 characters");
            });

            When(x => x.LastName != null, () =>
            {
                RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage("Last name cannot be empty")
                    .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");
            });

            When(x => x.Phone != null, () =>
            {
                RuleFor(x => x.Phone)
                    .NotEmpty().WithMessage("Phone cannot be empty")
                    .MaximumLength(25).WithMessage("Phone must not exceed 25 characters");
            });

            When(x => x.Email != null, () =>
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email cannot be empty")
                    .EmailAddress().WithMessage("Email is not valid")
                    .MaximumLength(50).WithMessage("Email must not exceed 50 characters");
            });

            RuleFor(x => x.NationalityCountryId)
                .GreaterThan(0).WithMessage("Nationality (country) is required")
                .When(x => x.NationalityCountryId.HasValue);

            RuleFor(x => x.ResidentialCityId)
                .GreaterThan(0).WithMessage("Residential city is required")
                .When(x => x.ResidentialCityId.HasValue);

            RuleFor(x => x.DateOfBirth)
                .Must(d => d!.Value < DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Date of birth must be in the past")
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.YearsOfExperiance)
                .InclusiveBetween(0, 70).WithMessage("Years of experience must be between 0 and 70")
                .When(x => x.YearsOfExperiance.HasValue);

            When(x => x.Bio != null, () =>
            {
                RuleFor(x => x.Bio)
                    .NotEmpty().WithMessage("Bio cannot be empty")
                    .MaximumLength(1000).WithMessage("Bio must not exceed 1000 characters");
            });

            When(x => x.NationalNumber != null, () =>
            {
                RuleFor(x => x.NationalNumber)
                    .NotEmpty().WithMessage("National number cannot be empty")
                    .MaximumLength(50).WithMessage("National number must not exceed 50 characters");
            });

            RuleFor(x => x.PassportNumber)
                .MaximumLength(50).WithMessage("Passport number must not exceed 50 characters")
                .When(x => x.PassportNumber != null);

            RuleFor(x => x.Languages)
                .MaximumLength(250).WithMessage("Languages must not exceed 250 characters")
                .When(x => x.Languages != null);
        }
    }
}
