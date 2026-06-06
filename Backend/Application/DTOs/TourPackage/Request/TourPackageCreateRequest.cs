using System;
using System.Collections.Generic;
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

        /// <summary>Destination country id (البلد / الوجهة).</summary>
        public int CountryId { get; set; }

        /// <summary>Cities/regions visited (المناطق اللي رح تنزار).</summary>
        public List<int> CityIds { get; set; } = new();

        /// <summary>Subscription price per person (تكلفة اشتراك البرنامج).</summary>
        public decimal PricePerPerson { get; set; }

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

        /// <summary>Tour guide name/notes (الدليل السياحي).</summary>
        public string TourGuide { get; set; } = null!;

        /// <summary>Publish immediately (نشر البرنامج).</summary>
        public bool IsPublished { get; set; }

        /// <summary>Main cover image (صورة البرنامج الرئيسية).</summary>
        public IFormFile? MainImage { get; set; }

        /// <summary>Full day-by-day itinerary (التفاصيل الكاملة للرحلة).</summary>
        public List<TourPackageDayRequest> Days { get; set; } = new();
    }
}
