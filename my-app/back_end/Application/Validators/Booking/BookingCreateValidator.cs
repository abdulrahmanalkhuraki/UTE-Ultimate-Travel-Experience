using Application;
using Application.DTOs.Booking.Request;
using Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Booking
{
    public sealed class BookingCreateValidator : AbstractValidator<BookingCreateRequest>
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public BookingCreateValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.PackageId)
                .GreaterThan(0).WithMessage(_localizer["PackageId must be a positive number"]);

            RuleFor(x => x.CompanionIds)
                .NotNull().WithMessage(_localizer["Companion list is required"]);

            RuleForEach(x => x.CompanionIds)
                .GreaterThan(0).WithMessage(_localizer["Each companion ID must be a positive number"]);

            RuleFor(x => x.FlightCabinClass)
                .IsInEnum().WithMessage(_localizer["Invalid Flight Cabin Class"]);

            RuleFor(x => x.RoomTypePreference)
                .MaximumLength(200).WithMessage(_localizer["Room type preference cannot exceed 200 characters"])
                .When(x => !string.IsNullOrWhiteSpace(x.RoomTypePreference));

            RuleFor(x => x.DietaryRequirements)
                .MaximumLength(200).WithMessage(_localizer["Dietary requirements cannot exceed 200 characters"])
                .When(x => !string.IsNullOrWhiteSpace(x.DietaryRequirements));

            RuleFor(x => x.SpecialRequests)
                .MaximumLength(200).WithMessage(_localizer["Special requests cannot exceed 200 characters"])
                .When(x => !string.IsNullOrWhiteSpace(x.SpecialRequests));
        }
    }
}
