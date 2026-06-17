namespace Domain.Entities;

/// <summary>
/// A single free-text activity inside a program day (النشاط).
/// Despite the legacy table name, this no longer links to an existing
/// <see cref="Attraction"/>; the tour company types the activity directly.
/// </summary>
public partial class Activity : BaseEntity
{
    /// <summary>Display order of the activity within the day (1-based).</summary>
    public int OrderNumber { get; set; }

    /// <summary>Activity title (عنوان النشاط).</summary>
    public string Title { get; set; } = null!;

    /// <summary>Short description of the activity (شرح مختصر عن النشاط).</summary>
    public string? Description { get; set; }

    /// <summary>Optional activity image URL.</summary>
    public string? ImageUrl { get; set; }

    /// <summary>Activity start time (من).</summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>Activity end time (إلى).</summary>
    public TimeOnly EndTime { get; set; }

    public int ItineraryId { get; set; }

    public virtual Itinerary Itinerary { get; set; } = null!;
}
