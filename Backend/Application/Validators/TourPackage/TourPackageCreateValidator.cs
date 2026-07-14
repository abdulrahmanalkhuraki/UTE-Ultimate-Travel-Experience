using System.Linq;
using Application.DTOs.TourPackage.Request;
using FluentValidation;

namespace Domain.Validators
{
    public class TourPackageCreateValidator : AbstractValidator<TourPackageCreateRequest>
    {
        public TourPackageCreateValidator()
        {
            RuleFor(x => x.PackageName)
                .NotEmpty().WithMessage("Program name is required")
                .MaximumLength(100).WithMessage("Program name must not exceed 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

            RuleFor(x => x.MeetingPoint)
                .NotEmpty().WithMessage("Meeting point is required")
                .MaximumLength(200).WithMessage("Meeting point must not exceed 200 characters");

            RuleFor(x => x.CountryId)
                .GreaterThan(0).WithMessage("A destination country is required");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required")
                .MaximumLength(10).WithMessage("Currency must not exceed 10 characters");

            RuleFor(x => x.DurationInDays)
                .GreaterThan(0).WithMessage("Duration must be greater than 0");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required")
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("End date must be on or after the start date");

            RuleFor(x => x.RegistrationDeadline)
                .NotEmpty().WithMessage("Registration deadline is required")
                .LessThanOrEqualTo(x => x.StartDate)
                .WithMessage("Registration deadline must be on or before the start date");

            RuleFor(x => x.TotalCapacity)
                .GreaterThan(0).WithMessage("Number of seats must be greater than 0");

            RuleFor(x => x.TouristGuideIds)
                .NotEmpty().WithMessage("At least one tour guide is required");
            RuleForEach(x => x.TouristGuideIds)
                .GreaterThan(0).WithMessage("Tour guide id must be greater than 0");

            RuleFor(x => x.ServiceLevel)
                .IsInEnum().WithMessage("Invalid service level");

            // When cabin classes are provided, validate them
            When(x => x.CabinClasses is { Count: > 0 }, () =>
            {
                RuleFor(x => x.CabinClasses)
                    .Must(list => list!.Count(c => c.IsDefault) == 1)
                    .WithMessage("Exactly one cabin class must be marked as the default");

                RuleFor(x => x.CabinClasses)
                    .Must(list => list!.Select(c => c.CabinClass).Distinct().Count() == list!.Count)
                    .WithMessage("Duplicate cabin classes are not allowed");

                RuleForEach(x => x.CabinClasses)
                    .SetValidator(new CabinClassRequestValidator());
            });

            // When no cabin classes, PricePerPerson is used directly
            When(x => x.CabinClasses is null or { Count: 0 }, () =>
            {
                RuleFor(x => x.PricePerPerson)
                    .GreaterThanOrEqualTo(0).WithMessage("Price per person must not be negative")
                    .LessThan(1000000).WithMessage("Price per person must be less than 1,000,000");
            });

            RuleFor(x => x.Media)
                .NotEmpty().WithMessage("At least one media file is required");

            RuleForEach(x => x.Media)
                .SetValidator(new TourPackageMediaCreateValidator());

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
