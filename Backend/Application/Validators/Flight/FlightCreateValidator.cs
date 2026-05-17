using FluentValidation;
using Application.DTOs.Flight.Request;

namespace Domain.Validators
{
    public class FlightCreateValidator : AbstractValidator<FlightCreateRequest>
    {
        public FlightCreateValidator()
        {
            RuleFor(x => x.FlightNumber)
                .NotEmpty().WithMessage("Flight number is required")
                .MaximumLength(20).WithMessage("Flight number must not exceed 20 characters")
                .Matches(@"^[A-Z0-9]{2,10}$").WithMessage("Flight number must contain only uppercase letters and numbers (e.g., AA1234)");

            RuleFor(x => x.Airline)
                .NotEmpty().WithMessage("Airline is required")
                .MaximumLength(100).WithMessage("Airline name must not exceed 100 characters");

            RuleFor(x => x.DepartureCityId)
                .GreaterThan(0).WithMessage("Departure city ID must be greater than 0")
                .NotEqual(x => x.ArrivalCityId).WithMessage("Departure and arrival cities cannot be the same");

            RuleFor(x => x.ArrivalCityId)
                .GreaterThan(0).WithMessage("Arrival city ID must be greater than 0");

            RuleFor(x => x.Departure)
                .NotEmpty().WithMessage("Departure time is required")
                .GreaterThan(DateTime.Now).WithMessage("Departure time must be in the future");

            RuleFor(x => x.Arrival)
                .NotEmpty().WithMessage("Arrival time is required")
                .GreaterThan(x => x.Departure).WithMessage("Arrival time must be after departure time");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .LessThan(1000000).WithMessage("Price must be less than 1,000,000");

            RuleFor(x => x)
                .Must(x => (x.Arrival - x.Departure).TotalHours <= 24)
                .WithMessage("Flight duration cannot exceed 24 hours")
                .Must(x => (x.Arrival - x.Departure).TotalMinutes >= 30)
                .WithMessage("Flight duration must be at least 30 minutes");
        }
    }
}