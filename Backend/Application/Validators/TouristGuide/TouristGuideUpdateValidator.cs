using System;
using Application.DTOs.TouristGuide.Request;
using FluentValidation;

namespace Application.Validators.TouristGuide
{
    /// <summary>
    /// Update-time rules for a tour guide. This is a PARTIAL update (تعديل جزئي):
    /// every field is optional, so each rule only runs when its field is actually
    /// sent (non-null). Images can be kept by sending nothing, so they are not
    /// required here (unlike create).
    /// </summary>
    public sealed class TouristGuideUpdateValidator : AbstractValidator<TouristGuideUpdateRequest>
    {
        public TouristGuideUpdateValidator()
        {
            When(x => x.Firstname != null, () =>
            {
                RuleFor(x => x.Firstname)
                    .NotEmpty().WithMessage("First name cannot be empty")
                    .MaximumLength(100).WithMessage("First name must not exceed 100 characters");
            });

            When(x => x.Lastname != null, () =>
            {
                RuleFor(x => x.Lastname)
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

            When(x => x.PlaceOfResidence != null, () =>
            {
                RuleFor(x => x.PlaceOfResidence)
                    .NotEmpty().WithMessage("Place of residence cannot be empty")
                    .MaximumLength(100).WithMessage("Place of residence must not exceed 100 characters");
            });

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

            RuleFor(x => x.CurrentLocation)
                .MaximumLength(100).WithMessage("Current location must not exceed 100 characters")
                .When(x => x.CurrentLocation != null);

            RuleFor(x => x.Languages)
                .MaximumLength(250).WithMessage("Languages must not exceed 250 characters")
                .When(x => x.Languages != null);
        }
    }
}
