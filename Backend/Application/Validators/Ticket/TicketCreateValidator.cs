using Application.DTOs.Ticket.Request;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Validators.Ticket
{
    public sealed class TicketCreateValidator : AbstractValidator<TicketCreateRequest>
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageBytes = 5 * 1024 * 1024;

        public TicketCreateValidator()
        {
            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required")
                .MaximumLength(200).WithMessage("Subject must not exceed 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

            RuleFor(x => x.Image)
                .Must(IsValidImage).WithMessage("Image must be JPG/PNG/WEBP and at most 5 MB")
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
