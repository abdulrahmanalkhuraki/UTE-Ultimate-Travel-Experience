using System.Linq;
using Application;
using Application.DTOs.TourPackage.Request;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Domain.Validators
{
    public class TourPackageUpdateValidator : AbstractValidator<TourPackageUpdateRequest>
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public TourPackageUpdateValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.PackageName)
                .NotEmpty().WithMessage(_localizer["Program name cannot be empty"])
                .MaximumLength(100).WithMessage(_localizer["Program name must not exceed 100 characters"])
                .When(x => x.PackageName != null);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(_localizer["Description cannot be empty"])
                .MaximumLength(1000).WithMessage(_localizer["Description must not exceed 1000 characters"])
                .When(x => x.Description != null);

            RuleFor(x => x.MeetingPoint)
                .NotEmpty().WithMessage(_localizer["Meeting point cannot be empty"])
                .MaximumLength(200).WithMessage(_localizer["Meeting point must not exceed 200 characters"])
                .When(x => x.MeetingPoint != null);

            RuleFor(x => x.CountryId)
                .GreaterThan(0).WithMessage(_localizer["A destination country is required"])
                .When(x => x.CountryId.HasValue);

            RuleFor(x => x.PricePerPerson)
                .GreaterThanOrEqualTo(0).WithMessage(_localizer["Price per person must not be negative"])
                .LessThan(1000000).WithMessage(_localizer["Price per person must be less than 1,000,000"])
                .When(x => x.PricePerPerson.HasValue);

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage(_localizer["Currency cannot be empty"])
                .MaximumLength(10).WithMessage(_localizer["Currency must not exceed 10 characters"])
                .When(x => x.Currency != null);

            RuleFor(x => x.DurationInDays)
                .GreaterThan(0).WithMessage(_localizer["Duration must be greater than 0"])
                .When(x => x.DurationInDays.HasValue);

            RuleFor(x => x)
                .Must(x => x.EndDate!.Value >= x.StartDate!.Value)
                .WithName(nameof(TourPackageUpdateRequest.EndDate))
                .WithMessage(_localizer["End date must be on or after the start date"])
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

            RuleFor(x => x)
                .Must(x => x.RegistrationDeadline!.Value <= x.StartDate!.Value)
                .WithName(nameof(TourPackageUpdateRequest.RegistrationDeadline))
                .WithMessage(_localizer["Registration deadline must be on or before the start date"])
                .When(x => x.StartDate.HasValue && x.RegistrationDeadline.HasValue);

            RuleFor(x => x.TotalCapacity)
                .GreaterThan(0).WithMessage(_localizer["Total capacity must be greater than 0"])
                .When(x => x.TotalCapacity.HasValue);

            RuleFor(x => x.TouristGuideIds)
                .NotEmpty().WithMessage(_localizer["At least one tour guide is required"])
                .When(x => x.TouristGuideIds != null);

            RuleForEach(x => x.TouristGuideIds)
                .GreaterThan(0).WithMessage(_localizer["Tour guide id must be greater than 0"])
                .When(x => x.TouristGuideIds != null);

            RuleFor(x => x.ServiceLevel)
                .IsInEnum().WithMessage(_localizer["Invalid service level"])
                .When(x => x.ServiceLevel.HasValue);

            // When cabin classes are provided, validate them
            When(x => x.CabinClasses is { Count: > 0 }, () =>
            {
                RuleFor(x => x.CabinClasses)
                    .Must(list => list!.Count(c => c.IsDefault) == 1)
                    .WithMessage(_localizer["Exactly one cabin class must be marked as the default"]);

                RuleFor(x => x.CabinClasses)
                    .Must(list => list!.Select(c => c.CabinClass).Distinct().Count() == list!.Count)
                    .WithMessage(_localizer["Duplicate cabin classes are not allowed"]);

                RuleForEach(x => x.CabinClasses)
                    .SetValidator(new CabinClassRequestValidator(localizer));
            });

            // When cabin classes are explicitly set to empty, PricePerPerson is needed
            When(x => x.CabinClasses is { Count: 0 }, () =>
            {
                RuleFor(x => x.PricePerPerson)
                    .NotNull().WithMessage(_localizer["Price per person is required when removing all cabin classes"]);
            });

            RuleFor(x => x.Days)
                .NotEmpty().WithMessage(_localizer["At least one day is required"])
                .When(x => x.Days != null);

            RuleForEach(x => x.Days)
                .SetValidator(new TourPackageDayValidator(localizer))
                .When(x => x.Days != null);

            RuleFor(x => x)
                .Must(x => x.Days!.Count == x.DurationInDays!.Value)
                .WithName(nameof(TourPackageUpdateRequest.Days))
                .WithMessage(_localizer["The number of days must match the trip duration"])
                .When(x => x.Days != null && x.DurationInDays is > 0);

            RuleFor(x => x.Media)
                .NotEmpty().WithMessage(_localizer["At least one media file is required"])
                .When(x => x.Media != null);

            RuleForEach(x => x.Media)
                .SetValidator(new TourPackageMediaCreateValidator(localizer))
                .When(x => x.Media != null);

            RuleForEach(x => x.ExistingMedia)
                .SetValidator(new TourPackageMediaUpdateValidator(localizer))
                .When(x => x.ExistingMedia != null);
        }
    }
}
