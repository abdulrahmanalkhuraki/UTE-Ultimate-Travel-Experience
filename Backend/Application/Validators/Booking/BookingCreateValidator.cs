using Application.DTOs.Booking.Request;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.Booking
{
    public sealed class BookingCreateValidator : AbstractValidator<BookingCreateRequest>
    {
        public BookingCreateValidator()
        {
            RuleFor(x => x.PackageId)
                .GreaterThan(0).WithMessage("PackageId must be a positive number");

            RuleFor(x => x.CompanionIds)
                .NotNull().WithMessage("Companion list is required");

            RuleForEach(x => x.CompanionIds)
                .GreaterThan(0).WithMessage("Each companion ID must be a positive number");

            RuleFor(x => x.FlightCabinClass)
                .IsInEnum().WithMessage("Invalid Flight Cabin Class");

            RuleFor(x => x.RoomTypePreference)
                .MaximumLength(200).WithMessage("Room type preference cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.RoomTypePreference));

            RuleFor(x => x.DietaryRequirements)
                .MaximumLength(200).WithMessage("Dietary requirements cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.DietaryRequirements));

            RuleFor(x => x.SpecialRequests)
                .MaximumLength(200).WithMessage("Special requests cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.SpecialRequests));
        }
    }
}
