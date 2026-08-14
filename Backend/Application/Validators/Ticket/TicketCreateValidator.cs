using Application;
using Application.DTOs.Ticket.Request;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Ticket
{
    public sealed class TicketCreateValidator : AbstractValidator<TicketCreateRequest>
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageBytes = 5 * 1024 * 1024;

        private readonly IStringLocalizer<SharedResource> _localizer;

        public TicketCreateValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage(_localizer["Subject is required"])
                .MaximumLength(200).WithMessage(_localizer["Subject must not exceed 200 characters"]);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(_localizer["Description is required"])
                .MaximumLength(2000).WithMessage(_localizer["Description must not exceed 2000 characters"]);

            RuleFor(x => x.Image)
                .Must(IsValidImage).WithMessage(_localizer["Image must be JPG/PNG/WEBP and at most 5 MB"])
                .When(x => x.Image != null);
        }

        private static bool IsValidImage(IFormFile? file)
        {
            if (file == null || file.Length == 0) return false;
            if (file.Length > MaxImageBytes) return false;

            var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            return ext != null && AllowedImageExtensions.Contains(ext);
        }
    }
}
