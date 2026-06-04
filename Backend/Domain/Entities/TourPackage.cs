using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TourPackage : BaseEntity
{
    public string PackageName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal PricePerPerson { get; set; }

    public int DurationInDays { get; set; }

    public int AvailableSeats { get; set; }

    public int CountryId { get; set; }

    public int CompanyId { get; set; }

    public string PickupLocation { get; set; } = null!;

    public virtual ICollection<PackageItinerary> PackageItineraries { get; set; } = new List<PackageItinerary>();

    public virtual ICollection<TourPackageHotel> TourPackageHotels { get; set; } = new List<TourPackageHotel>();

    public virtual ICollection<TourPackageFlight> TourPackageFlights { get; set; } = new List<TourPackageFlight>();

    public virtual Country Country { get; set; } = null!;

    public virtual TourCompany Company { get; set; } = null!;

    public virtual ICollection<Rate> Rates { get; set; } = new List<Rate>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
