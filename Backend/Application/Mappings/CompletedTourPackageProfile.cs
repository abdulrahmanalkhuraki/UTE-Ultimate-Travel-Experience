using Application.DTOs.TourPackage.Response;
using Application.Interfaces.TourPackage;
using Application.Mappings.Localization;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Mappings
{
    /// <summary>Mapping profile for CompletedTourPackageResponse DTO.</summary>
    public class CompletedTourPackageProfile : Profile
    {
        public CompletedTourPackageProfile()
        {
            CreateMap<TourPackage, CompletedTourPackageResponse>()
                .ForMember(d => d.PackageName,
                    o => o.MapFrom((src, _, _, ctx) => Localize.Pick(src.Translations, ctx, t => t.PackageName)))
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s =>
                    GetFirstImageUrl(s.Media)))
                .ForMember(d => d.NumberOfTouristsJoined, o => o.MapFrom(s =>
                    s.Bookings.Count(b => b.Status == BookingStatus.Confirmed)))
                .ForMember(d => d.NumberOfReviews, o => o.MapFrom(s =>
                    s.Reviews.Count))
                .ForMember(d => d.AverageRating, o => o.MapFrom(s =>
                    s.Rates.Count > 0 ? (float)s.Rates.Average(r => r.RateValue) : 0f))
                .ForMember(d => d.TotalEarnedAmount, o => o.MapFrom(s =>
                    CalculateNetEarnings(s.Bookings)));
        }

        /// <summary>
        /// Calculates net earnings from confirmed bookings after 5% platform commission.
        /// </summary>
        private static decimal CalculateNetEarnings(ICollection<Booking> bookings)
        {
            var confirmedTotal = bookings
                .Where(b => b.Status == BookingStatus.Completed)
                .Sum(b => b.TotalCost ?? 0m);

            return confirmedTotal * 0.95m; // 5% commission deducted
        }

        /// <summary>
        /// Gets the first image URL, falling back to any media type if no image exists.
        /// </summary>
        private static string? GetFirstImageUrl(ICollection<TourPackageMedia> media)
        {
            if (media == null || media.Count == 0)
                return null;

            var firstImage = media
                .OrderBy(m => m.DisplayOrder)
                .FirstOrDefault(m => m.MediaType == MediaType.Image);

            if (firstImage != null)
                return firstImage.MediaUrl;

            var firstMedia = media
                .OrderBy(m => m.DisplayOrder)
                .FirstOrDefault();

            return firstMedia?.MediaUrl;
        }
    }
}

