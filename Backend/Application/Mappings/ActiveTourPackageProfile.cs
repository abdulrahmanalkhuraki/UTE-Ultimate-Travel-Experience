using Application.DTOs.Booking.Response;
using Application.DTOs.TourPackage.Response;
using Application.Interfaces.TourPackage;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Mappings
{
    /// <summary>Mapping profile for ActiveTourPackageResponse DTO.</summary>
    public class ActiveTourPackageProfile : Profile
    {
        public ActiveTourPackageProfile()
        {
            CreateMap<Booking, BookingBriefResponse>()
                .ForMember(d => d.TouristName, o => o.MapFrom(s =>
                    s.User == null || s.User.Person == null ? string.Empty : s.User.Person.Fullname));

            CreateMap<TourPackage, ActiveTourPackageResponse>()
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s =>
                    GetFirstImageUrl(s.Media)))
                .ForMember(d => d.RemainingDaysUntilStart, o => o.MapFrom(s =>
                    CalculateDaysRemaining(s.StartDate)))
                .ForMember(d => d.RemainingDaysUntilRegistration, o => o.MapFrom(s =>
                    CalculateDaysRemaining(s.RegistrationDeadline)))
                .ForMember(d => d.AverageRating, o => o.MapFrom(s =>
                    s.Rates.Count > 0 ? (float)s.Rates.Average(r => r.RateValue) : 0f))
                .ForMember(d => d.RecentBookings, o => o.MapFrom(s =>
                    s.Bookings.OrderByDescending(b => b.CreatedAtUtc).Take(10).ToList()));
        }

        /// <summary>
        /// Calculates remaining days until a date, clamped to minimum 0.
        /// </summary>
        private static int CalculateDaysRemaining(DateOnly targetDate)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return Math.Max(0, targetDate.DayNumber - today.DayNumber);
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

