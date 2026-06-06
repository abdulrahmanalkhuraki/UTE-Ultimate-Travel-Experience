using Application.DTOs.TourPackage.Request;
using FluentValidation;

namespace Domain.Validators
{
    public class TourPackageDayValidator : AbstractValidator<TourPackageDayRequest>
    {
        public TourPackageDayValidator()
        {
            RuleFor(x => x.DayNumber)
                .GreaterThan(0).WithMessage("Day number must be greater than 0");

            RuleFor(x => x.DayTitle)
                .NotEmpty().WithMessage("Day title is required")
                .MaximumLength(100).WithMessage("Day title must not exceed 100 characters");

            RuleFor(x => x.DayDescription)
                .NotEmpty().WithMessage("Day description is required")
                .MaximumLength(500).WithMessage("Day description must not exceed 500 characters");

            RuleFor(x => x.Activities)
                .NotEmpty().WithMessage("Each day must have at least one activity");

            RuleForEach(x => x.Activities).SetValidator(new TourPackageActivityValidator());
        }
    }
}
