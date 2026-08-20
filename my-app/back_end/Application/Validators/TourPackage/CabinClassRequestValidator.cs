using Application;
using Application.DTOs.TourPackage.Request;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Domain.Validators;

public class CabinClassRequestValidator : AbstractValidator<CabinClassRequest>
{
    private readonly IStringLocalizer<SharedResource> _localizer;

    public CabinClassRequestValidator(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;

        RuleFor(x => x.CabinClass).IsInEnum().WithMessage(_localizer["Invalid flight cabin class"]);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage(_localizer["Cabin class price must not be negative"])
            .LessThan(1000000).WithMessage(_localizer["Cabin class price must be less than 1,000,000"]);
    }
}
