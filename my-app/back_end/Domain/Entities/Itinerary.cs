using Domain.Entities.Translations;

namespace Domain.Entities;

/// <summary>One day of a tour program (اليوم الأول / الثاني ...).</summary>
public partial class Itinerary : BaseEntity
{
    /// <summary>Sequential day number within the program (1-based).</summary>
    public int DayNumber { get; set; }

    public int PackageId { get; set; }

    public virtual TourPackage Package { get; set; } = null!;

    public virtual ICollection<ItineraryTranslation> Translations { get; set; } = new List<ItineraryTranslation>();

    /// <summary>Activities planned for the day (الأنشطة).</summary>
    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
