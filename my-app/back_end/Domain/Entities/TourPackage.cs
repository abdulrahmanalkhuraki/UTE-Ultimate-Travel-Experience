using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Entities.Translations;
using Domain.Enums;

namespace Domain.Entities;

    public partial class TourPackage : BaseEntity
    {
    public decimal PricePerPerson { get; set; }

    public string Currency { get; set; } = "USD";

    public int DurationInDays { get; set; }

    public int TotalCapacity {  get; set; }

    public int AvailableSeats { get; set; }

    public int CountryId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public DateOnly RegistrationDeadline { get; set; }

    public ServiceLevel ServiceLevel { get; set; } = ServiceLevel.Economy;

    public TourPackageStatus Status { get; set; } = TourPackageStatus.Pending;

    public float Rate {  get; set; }

    public string? RejectionReason { get; set; }

    public int PublishCount { get; set; }

    public DateTime? PublishedAtUtc { get; set; }

    /// <summary>Timestamp when package was cancelled (if applicable). Null for active/completed/rejected packages.</summary>
    public DateTime? CancelledAtUtc { get; set; }

    public bool IsDeleted { get; set; }

    public int CompanyId { get; set; }

    public virtual TourCompany Company { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<TourPackageTranslation> Translations { get; set; } = new List<TourPackageTranslation>();

    public virtual ICollection<TourPackage_Attraction> PackageAttractions { get; set; } = new List<TourPackage_Attraction>();

    public virtual ICollection<TourPackage_TouristGuide> TourPackageGuides { get; set; } = new List<TourPackage_TouristGuide>();

    public virtual ICollection<TourPackageCabinClass> CabinClasses { get; set; } = new List<TourPackageCabinClass>();

    public virtual ICollection<Itinerary> PackageItineraries { get; set; } = new List<Itinerary>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

    public virtual ICollection<Rate> Rates { get; set; } = new List<Rate>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<TourPackageMedia> Media { get; set; } = new List<TourPackageMedia>();

    /// <summary>Canonical (default-language) package name. Computed from translations.</summary>
    [NotMapped]
    public string PackageName => TranslationLookup.Default(Translations, t => t.PackageName, "Unknown package")!;

    /// <summary>Canonical (default-language) description. Computed from translations.</summary>
    [NotMapped]
    public string Description => TranslationLookup.Default(Translations, t => t.Description) ?? string.Empty;

    /// <summary>Canonical (default-language) meeting point. Computed from translations.</summary>
    [NotMapped]
    public string MeetingPoint => TranslationLookup.Default(Translations, t => t.MeetingPoint) ?? string.Empty;
}
