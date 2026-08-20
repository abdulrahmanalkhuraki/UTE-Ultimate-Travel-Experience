using Application;
using Application.DTOs.Booking.Request;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Booking
{
    public sealed class BookingUpdateValidator : AbstractValidator<BookingUpdateRequest>
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public BookingUpdateValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.RoomTypePreference)
                .MaximumLength(100).WithMessage(_localizer["Room type preference cannot exceed 100 characters"])
                .When(x => x.RoomTypePreference is not null);

            RuleFor(x => x.DietaryRequirements)
                .MaximumLength(500).WithMessage(_localizer["Dietary requirements cannot exceed 500 characters"])
                .When(x => x.DietaryRequirements is not null);

            RuleFor(x => x.SpecialRequests)
                .MaximumLength(500).WithMessage(_localizer["Special requests cannot exceed 500 characters"])
                .When(x => x.SpecialRequests is not null);

            RuleForEach(x => x.CompanionIds)
                .GreaterThan(0).WithMessage(_localizer["Each companion ID must be a positive number"]);

            RuleFor(x => x.FlightCabinClass)
                .IsInEnum().WithMessage(_localizer["Invalid Flight Cabin Class"]);
        }
    }
}
