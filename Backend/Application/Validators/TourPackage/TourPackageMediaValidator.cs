using Application.DTOs.TourPackage.Request;
using Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Domain.Validators
{
    public class TourPackageMediaValidator : AbstractValidator<MediaCreateRequest>
    {
        private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private static readonly string[] VideoExtensions = { ".mp4", ".mov", ".avi", ".wmv", ".flv", ".mkv" };

        private const long MaxImageSize = 5 * 1024 * 1024;
        private const long MaxVideoSize = 50 * 1024 * 1024;

        public TourPackageMediaValidator()
        {
            RuleFor(x => x.Media)
                .NotNull().WithMessage("Media file is required");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid media type");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order must not be negative");

            RuleFor(x => x.Media)
                .Must((request, file) => BeValidExtension(file, request.Type))
                .When(x => x.Media != null)
                .WithMessage((request, _) =>
                    request.Type == MediaType.Image
                        ? "Image must have a valid extension (jpg, jpeg, png, gif, webp)"
                        : "Video must have a valid extension (mp4, mov, avi, wmv, flv, mkv)");

            RuleFor(x => x.Media)
                .Must((request, file) => BeValidSize(file, request.Type))
                .When(x => x.Media != null)
                .WithMessage((request, _) =>
                    request.Type == MediaType.Image
                        ? "Image must be less than 5MB"
                        : "Video must be less than 50MB");
        }

        private static bool BeValidExtension(IFormFile? file, MediaType type)
        {
            if (file is null) return false;
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return type == MediaType.Image
                ? ImageExtensions.Contains(extension)
                : VideoExtensions.Contains(extension);
        }

        private static bool BeValidSize(IFormFile? file, MediaType type)
        {
            if (file is null) return false;
            var maxSize = type == MediaType.Image ? MaxImageSize : MaxVideoSize;
            return file.Length <= maxSize;
        }
    }
}
