using Application;
using Application.DTOs.SupportReply.Request;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.SupportReply
{
    public sealed class SupportReplyCreateValidator : AbstractValidator<SupportReplyCreateRequest>
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public SupportReplyCreateValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.ReplyContent)
                .NotEmpty().WithMessage(_localizer["Reply content is required"])
                .MaximumLength(2000).WithMessage(_localizer["Reply content must not exceed 2000 characters"]);
        }
    }
}
