using Domain.Enums;

namespace Domain.Entities;

    public partial class TourPackage : BaseEntity
    {
    public string PackageName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string MeetingPoint { get; set; } = null!;

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
