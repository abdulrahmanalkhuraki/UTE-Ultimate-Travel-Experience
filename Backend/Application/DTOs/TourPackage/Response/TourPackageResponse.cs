using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Application.DTOs.TourPackage.Response
{
    public class TourPackageResponse
    {
        public int Id { get; set; }

        public string PackageName { get; set; } = null!;

        public string? Description { get; set; }

        /// <summary>Default/base program price per person (التكلفة الافتراضية للبرنامج).</summary>
        public decimal PricePerPerson { get; set; }

        /// <summary>Economy flight class price (تكلفة الدرجة الاقتصادية).</summary>
        public decimal EconomyClassPrice { get; set; }

        /// <summary>Premium flight class price (تكلفة الدرجة المميزة).</summary>
        public decimal PremiumClassPrice { get; set; }

        /// <summary>Business flight class price (تكلفة درجة رجال الأعمال).</summary>
        public decimal BusinessClassPrice { get; set; }

        public string Currency { get; set; } = null!;

        public int DurationInDays { get; set; }

        public int AvailableSeats { get; set; }

        public int CountryId { get; set; }

        public string? CountryName { get; set; }

        public string? MainImageUrl { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public DateOnly RegistrationDeadline { get; set; }

        /// <summary>Guides assigned to this program (المرشدون المختارون).</summary>
        public List<TourPackageGuideResponse> Guides { get; set; } = new();

        /// <summary>Service level (مستوى الخدمة).</summary>
        public ServiceLevel ServiceLevel { get; set; }

        /// <summary>Arabic label for the service level (اسم مستوى الخدمة).</summary>
        public string ServiceLevelLabel => ServiceLevel switch
        {
            ServiceLevel.Economy => "خدمة اقتصادية",
            ServiceLevel.Standard => "خدمة عادية",
            ServiceLevel.Premium => "خدمة مميزة",
            ServiceLevel.FirstClass => "خدمة من الدرجة الأولى",
            _ => ServiceLevel.ToString()
        };

        /// <summary>Available flight cabin classes (تذاكر الطيران المتاحة). May be empty.</summary>
        public List<TourPackageCabinClassResponse> AvailableCabinClasses { get; set; } = new();

        public bool IsPublished { get; set; }

        /// <summary>Lifecycle status of the program (حالة البرنامج): Active or Cancelled.</summary>
        public TourPackageStatus Status { get; set; }

        /// <summary>Admin moderation state (حالة الموافقة): Pending, Accepted, or Rejected.</summary>
        public ProgramApprovalStatus ApprovalStatus { get; set; }

        /// <summary>Reason shown to the company when the program was rejected (سبب الرفض). Null unless rejected.</summary>
        public string? RejectionReason { get; set; }

        /// <summary>How many times the program has been published (كم مرة نُشر). 0 = never published.</summary>
        public int PublishCount { get; set; }

        /// <summary>When the program was most recently published (تاريخ آخر نشر). Null if never published.</summary>
        public DateTime? PublishedAtUtc { get; set; }

        public int CompanyId { get; set; }

        /// <summary>Name of the owning company (اسم الشركة). Useful for catalog and admin review.</summary>
        public string? CompanyName { get; set; }

        /// <summary>Cities/regions visited (المناطق).</summary>
        public List<PackageCityResponse> Cities { get; set; } = new();

        /// <summary>Full day-by-day itinerary.</summary>
        public List<TourPackageDayResponse> Days { get; set; } = new();

        public DateTime CreatedAtUtc { get; set; }

        // ===== Computed card fields (حقول البطاقة المحسوبة) =====
        // Derived from the stored dates at response time; not persisted.

        /// <summary>Days left until registration closes (باقي لانتهاء التسجيل). 0 once the deadline has passed.</summary>
        public int DaysUntilRegistrationDeadline => DaysFromToday(RegistrationDeadline);

        /// <summary>Days left until the trip starts (باقي لبدء الرحلة). 0 once it has started.</summary>
        public int DaysUntilStart => DaysFromToday(StartDate);

        /// <summary>How many days the program has been published (اديش صرلو منشور). Null if not currently published.</summary>
        public int? DaysSincePublished =>
            IsPublished && PublishedAtUtc is not null
                ? Math.Max(0, Today.DayNumber - DateOnly.FromDateTime(PublishedAtUtc.Value).DayNumber)
                : (int?)null;

        /// <summary>Arabic ordinal for the publish count (نص "المرة الأولى/الثانية/..."). Null if never published.</summary>
        public string? PublishLabel => FormatPublishOrdinal(PublishCount);

        private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);

        /// <summary>Whole days from today until <paramref name="date"/>, clamped at 0 for past dates.</summary>
        private static int DaysFromToday(DateOnly date) => Math.Max(0, date.DayNumber - Today.DayNumber);

        private static string? FormatPublishOrdinal(int count)
        {
            if (count <= 0)
                return null;

            string[] ordinals =
            {
                "الأولى", "الثانية", "الثالثة", "الرابعة", "الخامسة",
                "السادسة", "السابعة", "الثامنة", "التاسعة", "العاشرة"
            };

            return count <= ordinals.Length
                ? $"المرة {ordinals[count - 1]}"
                : $"المرة رقم {count}";
        }
    }
}
