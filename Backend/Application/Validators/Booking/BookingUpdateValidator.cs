using Application.DTOs.Booking.Request;
using FluentValidation;

namespace Application.Validators.Booking
{
    public sealed class BookingUpdateValidator : AbstractValidator<BookingUpdateRequest>
    {
        public BookingUpdateValidator()
        {
            RuleFor(x => x.RoomTypePreference)
                .MaximumLength(100).WithMessage("Room type preference cannot exceed 100 characters")
                .When(x => x.RoomTypePreference is not null);

            RuleFor(x => x.DietaryRequirements)
                .MaximumLength(500).WithMessage("Dietary requirements cannot exceed 500 characters")
                .When(x => x.DietaryRequirements is not null);

            RuleFor(x => x.SpecialRequests)
                .MaximumLength(500).WithMessage("Special requests cannot exceed 500 characters")
                .When(x => x.SpecialRequests is not null);

            RuleForEach(x => x.CompanionIds)
                .GreaterThan(0).WithMessage("Each companion ID must be a positive number");

            RuleFor(x => x.FlightCabinClass)
                .IsInEnum().WithMessage("Invalid Flight Cabin Class");
        }
    }
}
