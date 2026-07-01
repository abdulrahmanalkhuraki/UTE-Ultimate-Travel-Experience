using System.Linq;
using Application.DTOs.TourPackage.Request;
using FluentValidation;

namespace Domain.Validators
{
    /// <summary>
    /// Update-time rules. This is a PARTIAL update (تعديل جزئي): every field is
    /// optional, so each rule only runs when its field is actually sent (non-null).
    /// A field left out keeps its current value; a field sent must still be valid.
    /// </summary>
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

            RuleFor(x => x.CityIds)
                .NotEmpty().WithMessage("At least one region/city is required")
                .When(x => x.CityIds != null);

            RuleForEach(x => x.CityIds)
                .GreaterThan(0).WithMessage("City id must be greater than 0")
                .When(x => x.CityIds != null);

            RuleFor(x => x.PricePerPerson)
                .GreaterThanOrEqualTo(0).WithMessage("Price per person must not be negative")
                .LessThan(1000000).WithMessage("Price per person must be less than 1,000,000")
                .When(x => x.PricePerPerson.HasValue);

            RuleFor(x => x.EconomyClassPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Economy class price must not be negative")
                .LessThan(1000000).WithMessage("Economy class price must be less than 1,000,000")
                .When(x => x.EconomyClassPrice.HasValue);

            RuleFor(x => x.PremiumClassPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Premium class price must not be negative")
                .LessThan(1000000).WithMessage("Premium class price must be less than 1,000,000")
                .When(x => x.PremiumClassPrice.HasValue);

            RuleFor(x => x.BusinessClassPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Business class price must not be negative")
                .LessThan(1000000).WithMessage("Business class price must be less than 1,000,000")
                .When(x => x.BusinessClassPrice.HasValue);

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

            RuleFor(x => x.AvailableSeats)
                .GreaterThan(0).WithMessage("Number of seats must be greater than 0")
                .When(x => x.AvailableSeats.HasValue);

            RuleFor(x => x.TouristGuideIds)
                .NotEmpty().WithMessage("At least one tour guide is required")
                .When(x => x.TouristGuideIds != null);

            RuleForEach(x => x.TouristGuideIds)
                .GreaterThan(0).WithMessage("Tour guide id must be greater than 0")
                .When(x => x.TouristGuideIds != null);

            RuleFor(x => x.ServiceLevel)
                .IsInEnum().WithMessage("Invalid service level")
                .When(x => x.ServiceLevel.HasValue);

            RuleForEach(x => x.AvailableCabinClasses)
                .IsInEnum().WithMessage("Invalid flight cabin class")
                .When(x => x.AvailableCabinClasses != null);

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

            RuleForEach(x => x.NewMedia)
                .SetValidator(new TourPackageMediaValidator())
                .When(x => x.NewMedia != null);
        }
    }
}
