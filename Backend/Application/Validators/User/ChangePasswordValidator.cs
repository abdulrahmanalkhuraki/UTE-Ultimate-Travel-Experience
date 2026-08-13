using Application;
using Application.DTOs.User.Request;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.User
{
    public sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ChangePasswordValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage(_localizer["Current password is required to change the password"])
                .When(x => !string.IsNullOrWhiteSpace(x.NewPassword));

            RuleFor(x => x.NewPassword)
                .MinimumLength(8).WithMessage(_localizer["New password must be at least 8 characters"])
                .MaximumLength(100).WithMessage(_localizer["New password must not exceed 100 characters"])
                .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$")
                .WithMessage(_localizer["New password must contain at least one uppercase letter, one lowercase letter, and one digit"])
                .NotEqual(x => x.CurrentPassword).WithMessage(_localizer["New password must be different from the current password"])
                .When(x => !string.IsNullOrWhiteSpace(x.NewPassword));
        }
    }
}
