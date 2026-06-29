using Application.DTOs.SupportReply.Request;
using FluentValidation;

namespace Application.Validators.SupportReply
{
    public sealed class SupportReplyCreateValidator : AbstractValidator<SupportReplyCreateRequest>
    {
        public SupportReplyCreateValidator()
        {
            RuleFor(x => x.ReplyContent)
                .NotEmpty().WithMessage("Reply content is required")
                .MaximumLength(2000).WithMessage("Reply content must not exceed 2000 characters");
        }
    }
}
