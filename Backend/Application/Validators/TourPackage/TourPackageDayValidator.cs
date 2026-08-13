using Application;
using Application.DTOs.TourPackage.Request;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Domain.Validators
{
    public class TourPackageDayValidator : AbstractValidator<TourPackageDayRequest>
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public TourPackageDayValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.DayNumber)
                .GreaterThan(0).WithMessage(_localizer["Day number must be greater than 0"]);

            RuleFor(x => x.DayTitle)
                .NotEmpty().WithMessage(_localizer["Day title is required"])
                .MaximumLength(100).WithMessage(_localizer["Day title must not exceed 100 characters"]);

            RuleFor(x => x.DayDescription)
                .NotEmpty().WithMessage(_localizer["Day description is required"])
                .MaximumLength(500).WithMessage(_localizer["Day description must not exceed 500 characters"]);

            RuleFor(x => x.Activities)
                .NotEmpty().WithMessage(_localizer["Each day must have at least one activity"]);

            RuleForEach(x => x.Activities).SetValidator(new TourPackageActivityValidator(localizer));
        }
    }
}
