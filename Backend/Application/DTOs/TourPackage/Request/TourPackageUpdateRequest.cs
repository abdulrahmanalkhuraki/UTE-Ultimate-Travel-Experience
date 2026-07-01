using System;
using System.Collections.Generic;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.TourPackage.Request
{
    /// <summary>
    /// Update request for a tour program. Sent as multipart/form-data.
    /// <para>
    /// PARTIAL UPDATE (تعديل جزئي): every field is OPTIONAL. Only the fields that
    /// are actually sent are changed; anything left out keeps its current value.
    /// That is why all members are nullable and the collections are not
    /// initialized — <c>null</c> means "not sent, don't touch", while a sent value
    /// (even an empty list) replaces the current one.
    /// </para>
    /// To keep the existing cover image, just leave both <see cref="MainImage"/> and
    /// <see cref="MainImageUrl"/> empty.
    /// </summary>
    public class TourPackageUpdateRequest
    {
        public string? PackageName { get; set; }

        public string? Description { get; set; }

        /// <summary>Where the company meets the tourists (مكان الالتقاء مع السياح). Optional on update.</summary>
        public string? MeetingPoint { get; set; }

        public int? CountryId { get; set; }

        public List<int>? CityIds { get; set; }

        /// <summary>Default/base program price per person (التكلفة الافتراضية للبرنامج). Optional.</summary>
        public decimal? PricePerPerson { get; set; }

        /// <summary>Economy flight class price (تكلفة الدرجة الاقتصادية). Optional.</summary>
        public decimal? EconomyClassPrice { get; set; }

        /// <summary>Premium flight class price (تكلفة الدرجة المميزة). Optional.</summary>
        public decimal? PremiumClassPrice { get; set; }

        /// <summary>Business flight class price (تكلفة درجة رجال الأعمال). Optional.</summary>
        public decimal? BusinessClassPrice { get; set; }

        public string? Currency { get; set; }

        public int? DurationInDays { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public DateOnly? RegistrationDeadline { get; set; }

        public int? AvailableSeats { get; set; }

        /// <summary>Selected tour guides (المرشد السياحي). Optional; when sent, must contain at least one.</summary>
        public List<int>? TouristGuideIds { get; set; }

        /// <summary>Service level (مستوى الخدمة). Optional.</summary>
        public ServiceLevel? ServiceLevel { get; set; }

        /// <summary>Available flight cabin classes (تذاكر الطيران المتاحة). Optional;
        /// when sent empty it defaults to economy (الدرجة الاقتصادية).</summary>
        public List<FlightCabinClass>? AvailableCabinClasses { get; set; }

        public bool? IsPublished { get; set; }

        /// <summary>New media files to add (صور/فيديو جديدة). Optional.</summary>
        public List<MediaCreateRequest>? NewMedia { get; set; }

        /// <summary>Existing media to keep/update. Omitted items will be deleted. Optional.</summary>
        public List<MediaUpdateRequest>? ExistingMedia { get; set; }

        /// <summary>Full day-by-day itinerary. Optional; when sent it replaces the whole itinerary.</summary>
        public List<TourPackageDayRequest>? Days { get; set; }
    }
}
