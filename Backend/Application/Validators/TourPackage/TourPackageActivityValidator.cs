using Application.DTOs.TourPackage.Request;
using FluentValidation;

namespace Domain.Validators
{
    /// <summary>
    /// Validates a single activity. Shared by create and update: an image is
    /// required either as a new upload (<c>ProfileImage</c>) or as an existing URL
    /// (<c>ImageUrl</c>, used on update).
    /// </summary>
    public class TourPackageActivityValidator : AbstractValidator<TourPackageActivityRequest>
    {
        public TourPackageActivityValidator()
        {
            RuleFor(x => x.OrderNumber)
                .GreaterThan(0).WithMessage("Activity order number must be greater than 0");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Activity title is required")
                .MaximumLength(100).WithMessage("Activity title must not exceed 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Activity description is required")
                .MaximumLength(500).WithMessage("Activity description must not exceed 500 characters");

            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime)
                .WithMessage("Activity end time must be after its start time");

            RuleFor(x => x)
                .Must(a => a.Image != null || !string.IsNullOrWhiteSpace(a.ImageUrl))
                .WithMessage("Each activity must have an image");
        }
    }
}
