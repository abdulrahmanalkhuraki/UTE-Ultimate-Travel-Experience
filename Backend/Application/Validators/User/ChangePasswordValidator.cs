using Application.DTOs.User.Request;
using FluentValidation;

namespace Application.Validators.User
{
    public sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required to change the password")
                .When(x => !string.IsNullOrWhiteSpace(x.NewPassword));

            RuleFor(x => x.NewPassword)
                .MinimumLength(8).WithMessage("New password must be at least 8 characters")
                .MaximumLength(100).WithMessage("New password must not exceed 100 characters")
                .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$")
                .WithMessage("New password must contain at least one uppercase letter, one lowercase letter, and one digit")
                .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from the current password")
                .When(x => !string.IsNullOrWhiteSpace(x.NewPassword));
        }
    }
}
