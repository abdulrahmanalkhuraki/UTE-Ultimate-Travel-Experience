using System.Linq;
using Application;
using Application.DTOs.TourPackage.Request;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Domain.Validators
{
    public class TourPackageCreateValidator : AbstractValidator<TourPackageCreateRequest>
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public TourPackageCreateValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.PackageName)
                .NotEmpty().WithMessage(_localizer["Program name is required"])
                .MaximumLength(100).WithMessage(_localizer["Program name must not exceed 100 characters"]);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(_localizer["Description is required"])
                .MaximumLength(1000).WithMessage(_localizer["Description must not exceed 1000 characters"]);

            RuleFor(x => x.MeetingPoint)
                .NotEmpty().WithMessage(_localizer["Meeting point is required"])
                .MaximumLength(200).WithMessage(_localizer["Meeting point must not exceed 200 characters"]);

            RuleFor(x => x.CountryId)
                .GreaterThan(0).WithMessage(_localizer["A destination country is required"]);

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage(_localizer["Currency is required"])
                .MaximumLength(10).WithMessage(_localizer["Currency must not exceed 10 characters"]);

            RuleFor(x => x.DurationInDays)
                .GreaterThan(0).WithMessage(_localizer["Duration must be greater than 0"]);

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage(_localizer["Start date is required"]);

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage(_localizer["End date is required"])
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage(_localizer["End date must be on or after the start date"]);

            RuleFor(x => x.RegistrationDeadline)
                .NotEmpty().WithMessage(_localizer["Registration deadline is required"])
                .LessThanOrEqualTo(x => x.StartDate)
                .WithMessage(_localizer["Registration deadline must be on or before the start date"]);

            RuleFor(x => x.TotalCapacity)
                .GreaterThan(0).WithMessage(_localizer["Number of seats must be greater than 0"]);

            RuleFor(x => x.TouristGuideIds)
                .NotEmpty().WithMessage(_localizer["At least one tour guide is required"]);
            RuleForEach(x => x.TouristGuideIds)
                .GreaterThan(0).WithMessage(_localizer["Tour guide id must be greater than 0"]);

            RuleFor(x => x.ServiceLevel)
                .IsInEnum().WithMessage(_localizer["Invalid service level"]);

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

            // When no cabin classes, PricePerPerson is used directly
            When(x => x.CabinClasses is null or { Count: 0 }, () =>
            {
                RuleFor(x => x.PricePerPerson)
                    .GreaterThanOrEqualTo(0).WithMessage(_localizer["Price per person must not be negative"])
                    .LessThan(1000000).WithMessage(_localizer["Price per person must be less than 1,000,000"]);
            });

            RuleFor(x => x.Media)
                .NotEmpty().WithMessage(_localizer["At least one media file is required"]);

            RuleForEach(x => x.Media)
                .SetValidator(new TourPackageMediaCreateValidator(localizer));

            RuleFor(x => x.Days)
                .NotEmpty().WithMessage(_localizer["At least one day is required"]);

            RuleFor(x => x)
                .Must(x => x.Days.Count == x.DurationInDays)
                .When(x => x.DurationInDays > 0)
                .WithMessage(_localizer["The number of days must match the trip duration"]);

            RuleForEach(x => x.Days).SetValidator(new TourPackageDayValidator(localizer));
        }
    }
}
