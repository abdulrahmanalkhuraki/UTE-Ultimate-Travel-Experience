using Application;
using Application.DTOs.Review.Request;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Review
{
    public sealed class ReviewCreateValidator : AbstractValidator<ReviewCreateRequest>
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ReviewCreateValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(p => p.comment)
                .NotEmpty().WithMessage(_localizer["Comment is Required."])
                .MaximumLength(500).WithMessage(_localizer["Comment must not exceed 50 characters"]);
        }
    }
}
