using System.Linq;
using Application.DTOs.TourPackage.Request;
using FluentValidation;

namespace Domain.Validators
{
    public class TourPackageUpdateValidator : AbstractValidator<TourPackageUpdateRequest>
    {
        public TourPackageUpdateValidator()
        {
            RuleFor(x => x.PackageName)
                .NotEmpty().WithMessage("Program name cannot be empty")
                .MaximumLength(100).WithMessage("Program name must not exceed 100 characters")
                .When(x => x.PackageName != null);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description cannot be empty")
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
                .When(x => x.Description != null);

            RuleFor(x => x.MeetingPoint)
                .NotEmpty().WithMessage("Meeting point cannot be empty")
                .MaximumLength(200).WithMessage("Meeting point must not exceed 200 characters")
                .When(x => x.MeetingPoint != null);

            RuleFor(x => x.CountryId)
                .GreaterThan(0).WithMessage("A destination country is required")
                .When(x => x.CountryId.HasValue);

            RuleFor(x => x.PricePerPerson)
                .GreaterThanOrEqualTo(0).WithMessage("Price per person must not be negative")
                .LessThan(1000000).WithMessage("Price per person must be less than 1,000,000")
                .When(x => x.PricePerPerson.HasValue);

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency cannot be empty")
                .MaximumLength(10).WithMessage("Currency must not exceed 10 characters")
                .When(x => x.Currency != null);

            RuleFor(x => x.DurationInDays)
                .GreaterThan(0).WithMessage("Duration must be greater than 0")
                .When(x => x.DurationInDays.HasValue);

            RuleFor(x => x)
                .Must(x => x.EndDate!.Value >= x.StartDate!.Value)
                .WithName(nameof(TourPackageUpdateRequest.EndDate))
                .WithMessage("End date must be on or after the start date")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

            RuleFor(x => x)
                .Must(x => x.RegistrationDeadline!.Value <= x.StartDate!.Value)
                .WithName(nameof(TourPackageUpdateRequest.RegistrationDeadline))
                .WithMessage("Registration deadline must be on or before the start date")
                .When(x => x.StartDate.HasValue && x.RegistrationDeadline.HasValue);

            RuleFor(x => x.TotalCapacity)
                .GreaterThan(0).WithMessage("Total capacity must be greater than 0")
                .When(x => x.TotalCapacity.HasValue);

            RuleFor(x => x.TouristGuideIds)
                .NotEmpty().WithMessage("At least one tour guide is required")
                .When(x => x.TouristGuideIds != null);

            RuleForEach(x => x.TouristGuideIds)
                .GreaterThan(0).WithMessage("Tour guide id must be greater than 0")
                .When(x => x.TouristGuideIds != null);

            RuleFor(x => x.ServiceLevel)
                .IsInEnum().WithMessage("Invalid service level")
                .When(x => x.ServiceLevel.HasValue);

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

            // When cabin classes are explicitly set to empty, PricePerPerson is needed
            When(x => x.CabinClasses is { Count: 0 }, () =>
            {
                RuleFor(x => x.PricePerPerson)
                    .NotNull().WithMessage("Price per person is required when removing all cabin classes");
            });

            RuleFor(x => x.Days)
                .NotEmpty().WithMessage("At least one day is required")
                .When(x => x.Days != null);

            RuleForEach(x => x.Days)
                .SetValidator(new TourPackageDayValidator())
                .When(x => x.Days != null);

            RuleFor(x => x)
                .Must(x => x.Days!.Count == x.DurationInDays!.Value)
                .WithName(nameof(TourPackageUpdateRequest.Days))
                .WithMessage("The number of days must match the trip duration")
                .When(x => x.Days != null && x.DurationInDays is > 0);

            RuleFor(x => x.Media)
                .NotEmpty().WithMessage("At least one media file is required")
                .When(x => x.Media != null);

            RuleForEach(x => x.Media)
                .SetValidator(new TourPackageMediaCreateValidator())
                .When(x => x.Media != null);

            RuleForEach(x => x.ExistingMedia)
                .SetValidator(new TourPackageMediaUpdateValidator())
                .When(x => x.ExistingMedia != null);
        }
    }
}
