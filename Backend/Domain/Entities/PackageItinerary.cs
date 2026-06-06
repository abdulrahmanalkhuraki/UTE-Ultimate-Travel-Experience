using System;
using System.Collections.Generic;

namespace Domain.Entities;

/// <summary>One day of a tour program (اليوم الأول / الثاني ...).</summary>
public partial class PackageItinerary : BaseEntity
{
    /// <summary>Sequential day number within the program (1-based).</summary>
    public int DayNumber { get; set; }

    /// <summary>Day title (اليوم الأول).</summary>
    public string DayTitle { get; set; } = null!;

    /// <summary>Short description of the day (شرح مختصر عن هذا اليوم).</summary>
    public string? DayDescription { get; set; }

    public int PackageId { get; set; }

    public virtual TourPackage Package { get; set; } = null!;

    /// <summary>Activities planned for the day (الأنشطة).</summary>
    public virtual ICollection<PackageItineraryAttraction> PackageItineraryAttractions { get; set; } = new List<PackageItineraryAttraction>();
}
