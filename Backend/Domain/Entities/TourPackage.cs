using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities;

    public partial class TourPackage : BaseEntity
    {
    /// <summary>Program name (اسم البرنامج).</summary>
    public string PackageName { get; set; } = null!;

    /// <summary>Free-text description shown to tourists (وصف).</summary>
    public string? Description { get; set; }

    /// <summary>Where the company meets the tourists (مكان الالتقاء مع السياح). Required.</summary>
    public string MeetingPoint { get; set; } = null!;

    /// <summary>Default/base program price per person (التكلفة الافتراضية للبرنامج). Optional.</summary>
    public decimal PricePerPerson { get; set; }

    /// <summary>Economy flight class price (تكلفة الدرجة الاقتصادية). Optional.</summary>
    public decimal EconomyClassPrice { get; set; }

    /// <summary>Premium flight class price (تكلفة الدرجة المميزة). Optional.</summary>
    public decimal PremiumClassPrice { get; set; }

    /// <summary>Business flight class price (تكلفة درجة رجال الأعمال). Optional.</summary>
    public decimal BusinessClassPrice { get; set; }

    /// <summary>Currency code for <see cref="PricePerPerson"/> (العملة), e.g. USD, JOD.</summary>
    public string Currency { get; set; } = "USD";

    /// <summary>Trip length in days (مدة الرحلة).</summary>
    public int DurationInDays { get; set; }

    /// <summary>Number of seats offered (عدد الأشخاص).</summary>
    public int AvailableSeats { get; set; }

    /// <summary>Destination country (البلد / الوجهة).</summary>
    public int CountryId { get; set; }

    /// <summary>Trip start date (تاريخ بداية الرحلة).</summary>
    public DateOnly StartDate { get; set; }

    /// <summary>Trip end date (تاريخ نهاية الرحلة).</summary>
    public DateOnly EndDate { get; set; }

    /// <summary>Last day a tourist may register (تاريخ نهاية التسجيل).</summary>
    public DateOnly RegistrationDeadline { get; set; }

    /// <summary>Service level offered (مستوى الخدمة). Defaults to economy (الدرجة الاقتصادية).</summary>
    public ServiceLevel ServiceLevel { get; set; } = ServiceLevel.Economy;

    /// <summary>Whether the program is published and visible to tourists (نشر البرنامج).</summary>
    public bool IsPublished { get; set; }

    /// <summary>Lifecycle status of the program (حالة البرنامج). Defaults to active; set to cancelled when the company cancels it (ملغى).</summary>
    public TourPackageStatus Status { get; set; } = TourPackageStatus.Active;

    /// <summary>Admin moderation state (حالة الموافقة). New programs start pending until an admin accepts (مقبول) or rejects (مرفوض) them.</summary>
    public PackageApprovalStatus ApprovalStatus { get; set; } = PackageApprovalStatus.Pending;

    /// <summary>Reason shown to the company when an admin rejects the program (سبب الرفض). Set on reject, cleared on accept.</summary>
    public string? RejectionReason { get; set; }

    /// <summary>How many times this program has been published (كم مرة نُشر). Incremented on each unpublished→published transition; 1 means first time (المرة الأولى).</summary>
    public int PublishCount { get; set; }

    /// <summary>When the program was most recently published (تاريخ آخر نشر), used to show how long it has been published (اديش صرلو منشور). Null if never published.</summary>
    public DateTime? PublishedAtUtc { get; set; }

    public bool IsDeleted { get; set; }

    public int CompanyId { get; set; }

    public virtual TourCompany Company { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<TourPackage_Attraction> PackageAttractions { get; set; } = new List<TourPackage_Attraction>();

    public virtual ICollection<TourPackage_TouristGuide> TourPackageGuides { get; set; } = new List<TourPackage_TouristGuide>();

    public virtual ICollection<TourPackageCabinClass> CabinClasses { get; set; } = new List<TourPackageCabinClass>();

    public virtual ICollection<Itinerary> PackageItineraries { get; set; } = new List<Itinerary>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

    public virtual ICollection<Rate> Rates { get; set; } = new List<Rate>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<TourPackageMedia> Media { get; set; } = new List<TourPackageMedia>();
}
