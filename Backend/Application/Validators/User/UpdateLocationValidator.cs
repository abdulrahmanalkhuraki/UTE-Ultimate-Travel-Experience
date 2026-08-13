using Application;
using Application.DTOs.User.Request;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.User
{
    public sealed class UpdateLocationValidator : AbstractValidator<UpdateLocationRequest>
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public UpdateLocationValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage(_localizer["Latitude must be between -90 and 90 degrees."]);

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage(_localizer["Longitude must be between -180 and 180 degrees."]);
        }
    }
}
