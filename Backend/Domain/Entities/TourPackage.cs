using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TourPackage : BaseEntity
{
    /// <summary>Program name (اسم البرنامج).</summary>
    public string PackageName { get; set; } = null!;

    /// <summary>Free-text description shown to tourists (وصف).</summary>
    public string? Description { get; set; }

    /// <summary>Subscription price per person (تكلفة اشتراك البرنامج).</summary>
    public decimal PricePerPerson { get; set; }

    /// <summary>Currency code for <see cref="PricePerPerson"/> (العملة), e.g. USD, JOD.</summary>
    public string Currency { get; set; } = "USD";

    /// <summary>Trip length in days (مدة الرحلة).</summary>
    public int DurationInDays { get; set; }

    /// <summary>Number of seats offered (عدد الأشخاص).</summary>
    public int AvailableSeats { get; set; }

    /// <summary>Destination country (البلد / الوجهة).</summary>
    public int CountryId { get; set; }

    /// <summary>Main cover image of the program (صورة البرنامج الرئيسية).</summary>
    public string? MainImageUrl { get; set; }

    /// <summary>Trip start date (تاريخ بداية الرحلة).</summary>
    public DateOnly StartDate { get; set; }

    /// <summary>Trip end date (تاريخ نهاية الرحلة).</summary>
    public DateOnly EndDate { get; set; }

    /// <summary>Last day a tourist may register (تاريخ نهاية التسجيل).</summary>
    public DateOnly RegistrationDeadline { get; set; }

    /// <summary>Tour guide name/notes (الدليل السياحي).</summary>
    public string? TourGuide { get; set; }

    /// <summary>Whether the program is published and visible to tourists (نشر البرنامج).</summary>
    public bool IsPublished { get; set; }

    /// <summary>Owning tour company.</summary>
    public int CompanyId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual TourCompany Company { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    /// <summary>Regions/cities visited by this program (المناطق اللي رح تنزار).</summary>
    public virtual ICollection<PackageCity> PackageCities { get; set; } = new List<PackageCity>();

    public virtual ICollection<PackageItinerary> PackageItineraries { get; set; } = new List<PackageItinerary>();

    public virtual ICollection<TourPackageFlight> TourPackageFlights { get; set; } = new List<TourPackageFlight>();

    public virtual ICollection<TourPackageHotel> TourPackageHotels { get; set; } = new List<TourPackageHotel>();

    public virtual ICollection<Rate> Rates { get; set; } = new List<Rate>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
