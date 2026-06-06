using System.Linq;
using Application.DTOs.TourPackage.Request;
using FluentValidation;

namespace Domain.Validators
{
    /// <summary>
    /// Update-time rules. Mirrors create, except the main image may be kept by
    /// sending its existing URL instead of re-uploading a file.
    /// </summary>
    public class TourPackageUpdateValidator : AbstractValidator<TourPackageUpdateRequest>
    {
        public TourPackageUpdateValidator()
        {
            RuleFor(x => x.PackageName)
                .NotEmpty().WithMessage("Program name is required")
                .MaximumLength(100).WithMessage("Program name must not exceed 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

            RuleFor(x => x.CountryId)
                .GreaterThan(0).WithMessage("A destination country is required");

            RuleFor(x => x.CityIds)
                .NotEmpty().WithMessage("At least one region/city is required");
            RuleForEach(x => x.CityIds)
                .GreaterThan(0).WithMessage("City id must be greater than 0");

            RuleFor(x => x.PricePerPerson)
                .GreaterThan(0).WithMessage("Price per person must be greater than 0")
                .LessThan(1000000).WithMessage("Price per person must be less than 1,000,000");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required")
                .MaximumLength(10).WithMessage("Currency must not exceed 10 characters");

            RuleFor(x => x.DurationInDays)
                .GreaterThan(0).WithMessage("Duration must be greater than 0");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required")
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("End date must be on or after the start date");

            RuleFor(x => x.RegistrationDeadline)
                .NotEmpty().WithMessage("Registration deadline is required")
                .LessThanOrEqualTo(x => x.StartDate)
                .WithMessage("Registration deadline must be on or before the start date");

            RuleFor(x => x.AvailableSeats)
                .GreaterThan(0).WithMessage("Number of seats must be greater than 0");

            RuleFor(x => x.TourGuide)
                .NotEmpty().WithMessage("Tour guide is required")
                .MaximumLength(150).WithMessage("Tour guide must not exceed 150 characters");

            RuleFor(x => x)
                .Must(x => x.MainImage != null || !string.IsNullOrWhiteSpace(x.MainImageUrl))
                .WithMessage("Main program image is required");

            RuleFor(x => x.Days)
                .NotEmpty().WithMessage("At least one day is required");

            RuleFor(x => x)
                .Must(x => x.Days.Count == x.DurationInDays)
                .When(x => x.DurationInDays > 0)
                .WithMessage("The number of days must match the trip duration");

            RuleForEach(x => x.Days).SetValidator(new TourPackageDayValidator());
        }
    }
}
