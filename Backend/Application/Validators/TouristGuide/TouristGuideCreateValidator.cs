using System;
using Application.DTOs.TouristGuide.Request;
using FluentValidation;

namespace Application.Validators.TouristGuide
{
    /// <summary>
    /// Create-time rules for a tour guide ("إضافة مرشد" form). Core identity and
    /// contact fields are required; images and a few extras are optional.
    /// </summary>
    public sealed class TouristGuideCreateValidator : AbstractValidator<TouristGuideCreateRequest>
    {
        public TouristGuideCreateValidator()
        {
            RuleFor(x => x.Firstname)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

            RuleFor(x => x.Lastname)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .MaximumLength(25).WithMessage("Phone must not exceed 25 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is not valid")
                .MaximumLength(50).WithMessage("Email must not exceed 50 characters");

            RuleFor(x => x.NationalityCountryId)
                .GreaterThan(0).WithMessage("Nationality (country) is required");

            RuleFor(x => x.PlaceOfResidence)
                .NotEmpty().WithMessage("Place of residence is required")
                .MaximumLength(100).WithMessage("Place of residence must not exceed 100 characters");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("Date of birth must be in the past");

            RuleFor(x => x.YearsOfExperiance)
                .InclusiveBetween(0, 70).WithMessage("Years of experience must be between 0 and 70");

            RuleFor(x => x.Bio)
                .NotEmpty().WithMessage("Bio is required")
                .MaximumLength(1000).WithMessage("Bio must not exceed 1000 characters");

            RuleFor(x => x.NationalNumber)
                .NotEmpty().WithMessage("National number is required")
                .MaximumLength(50).WithMessage("National number must not exceed 50 characters");

            RuleFor(x => x.PassportNumber)
                .MaximumLength(50).WithMessage("Passport number must not exceed 50 characters");

            RuleFor(x => x.CurrentLocation)
                .MaximumLength(100).WithMessage("Current location must not exceed 100 characters");

            RuleFor(x => x.Languages)
                .MaximumLength(250).WithMessage("Languages must not exceed 250 characters");

            // Images are required when adding a guide (صور المرشد إجبارية).
            RuleFor(x => x.ProfileImage)
                .NotNull().WithMessage("Profile image is required");

            RuleFor(x => x.IdCardImage)
                .NotNull().WithMessage("National ID image is required");

            RuleFor(x => x.PassportImage)
                .NotNull().WithMessage("Passport image is required");
        }
    }
}
