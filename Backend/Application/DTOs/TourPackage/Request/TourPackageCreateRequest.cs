using System;
using System.Collections.Generic;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.TourPackage.Request
{
    /// <summary>
    /// Create request for a tour program ("نشر البرنامج" form).
    /// Sent as multipart/form-data because it carries the main image and the
    /// per-activity images. The owning company is resolved from the JWT,
    /// never from the client. All fields are required (see the validator).
    /// </summary>
    public class TourPackageCreateRequest
    {
        /// <summary>Program name (اسم البرنامج).</summary>
        public string PackageName { get; set; } = null!;

        /// <summary>Description (وصف).</summary>
        public string Description { get; set; } = null!;

        /// <summary>Where the company meets the tourists (مكان الالتقاء مع السياح). Required.</summary>
        public string MeetingPoint { get; set; } = null!;

        /// <summary>Destination country id (البلد / الوجهة).</summary>
        public int CountryId { get; set; }

        /// <summary>Cities/regions visited (المناطق اللي رح تنزار).</summary>
        public List<int> CityIds { get; set; } = new();

        /// <summary>Default/base program price per person (التكلفة الافتراضية للبرنامج). Optional — null/omitted is stored as 0.</summary>
        public decimal? PricePerPerson { get; set; }

        /// <summary>Economy flight class price (تكلفة الدرجة الاقتصادية). Optional — null/omitted is stored as 0.</summary>
        public decimal? EconomyClassPrice { get; set; }

        /// <summary>Premium flight class price (تكلفة الدرجة المميزة). Optional — null/omitted is stored as 0.</summary>
        public decimal? PremiumClassPrice { get; set; }

        /// <summary>Business flight class price (تكلفة درجة رجال الأعمال). Optional — null/omitted is stored as 0.</summary>
        public decimal? BusinessClassPrice { get; set; }

        /// <summary>Currency code (العملة), e.g. USD, JOD.</summary>
        public string Currency { get; set; } = "USD";

        /// <summary>Trip length in days (مدة الرحلة).</summary>
        public int DurationInDays { get; set; }

        /// <summary>Trip start date (تاريخ بداية الرحلة).</summary>
        public DateOnly StartDate { get; set; }

        /// <summary>Trip end date (تاريخ نهاية الرحلة).</summary>
        public DateOnly EndDate { get; set; }

        /// <summary>Registration deadline (تاريخ نهاية التسجيل).</summary>
        public DateOnly RegistrationDeadline { get; set; }

        /// <summary>Number of seats (عدد الأشخاص).</summary>
        public int AvailableSeats { get; set; }

        /// <summary>
        /// Selected tour guides (المرشد السياحي). One or more guides chosen from the
        /// company's own guides. At least one is required.
        /// </summary>
        public List<int> TouristGuideIds { get; set; } = new();

        /// <summary>Service level (مستوى الخدمة). Defaults to economy (الدرجة الاقتصادية).</summary>
        public ServiceLevel ServiceLevel { get; set; } = ServiceLevel.Economy;

        /// <summary>
        /// Available flight cabin classes (تذاكر الطيران المتاحة). Multi-valued.
        /// Defaults to economy (الدرجة الاقتصادية) when none are sent.
        /// </summary>
        public List<FlightCabinClass> AvailableCabinClasses { get; set; } = new();

        /// <summary>Publish immediately (نشر البرنامج).</summary>
        public bool IsPublished { get; set; }

        /// <summary>Main cover image (صورة البرنامج الرئيسية).</summary>
        public IFormFile? MainImage { get; set; }

        /// <summary>Full day-by-day itinerary (التفاصيل الكاملة للرحلة).</summary>
        public List<TourPackageDayRequest> Days { get; set; } = new();
    }
}
