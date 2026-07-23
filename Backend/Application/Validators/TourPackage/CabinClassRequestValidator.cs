using Application.DTOs.TourPackage.Request;
using FluentValidation;

namespace Domain.Validators;

public class CabinClassRequestValidator : AbstractValidator<CabinClassRequest>
{
    public CabinClassRequestValidator()
    {
        RuleFor(x => x.CabinClass).IsInEnum().WithMessage("Invalid flight cabin class");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Cabin class price must not be negative")
            .LessThan(1000000).WithMessage("Cabin class price must be less than 1,000,000");
    }
}
