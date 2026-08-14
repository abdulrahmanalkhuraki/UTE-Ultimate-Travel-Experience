using Application;
using Application.DTOs.TourPackage.Request;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Domain.Validators
{
    /// <summary>
    /// Validates a single activity. Shared by create and update: an image is
    /// required either as a new upload (<c>ProfileImage</c>) or as an existing URL
    /// (<c>ImageUrl</c>, used on update).
    /// </summary>
    public class TourPackageActivityValidator : AbstractValidator<TourPackageActivityRequest>
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public TourPackageActivityValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.OrderNumber)
                .GreaterThan(0).WithMessage(_localizer["Activity order number must be greater than 0"]);

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(_localizer["Activity title is required"])
                .MaximumLength(100).WithMessage(_localizer["Activity title must not exceed 100 characters"]);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(_localizer["Activity description is required"])
                .MaximumLength(500).WithMessage(_localizer["Activity description must not exceed 500 characters"]);

            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime)
                .WithMessage(_localizer["Activity end time must be after its start time"]);

            RuleFor(x => x)
                .Must(a => a.Image != null || !string.IsNullOrWhiteSpace(a.ImageUrl))
                .WithMessage(_localizer["Each activity must have an image"]);
        }
    }
}
