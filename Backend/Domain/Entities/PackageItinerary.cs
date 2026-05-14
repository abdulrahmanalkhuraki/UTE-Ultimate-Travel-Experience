using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class PackageItinerary : BaseEntity
{
    public int DayNumber { get; set; }

    public string DayTitle { get; set; } = null!;

    public int PackageId { get; set; }

    public virtual TourPackage Package { get; set; } = null!;

    public virtual ICollection<PackageItineraryAttraction> PackageItineraryAttractions { get; set; } = new List<PackageItineraryAttraction>();
}
