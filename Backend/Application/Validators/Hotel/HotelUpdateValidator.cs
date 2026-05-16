using Application.DTOs.Hotel.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Hotel
{
    public sealed class HotelUpdateValidator : AbstractValidator<HotelUpdateRequest>
    {
        public HotelUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid hotel ID");

            RuleFor(x => x.HotelName)
                .NotEmpty().WithMessage("Hotel name is required")
                .MaximumLength(100).WithMessage("Hotel name cannot exceed 100 characters")
                .MinimumLength(3).WithMessage("Hotel name must be at least 3 characters");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");

            RuleFor(x => x.StarRating)
                .InclusiveBetween(1, 5).WithMessage("Star rating must be between 1 and 5");

            RuleFor(x => x.PricePerNight)
                .GreaterThan(0).WithMessage("PricePerNight must be non-negative.");
        }
    }
}
