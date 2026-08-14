using Domain.Entities.Translations;

namespace Domain.Entities;

public partial class Activity : BaseEntity
{
    public int OrderNumber { get; set; }

    public string? ImageUrl { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int ItineraryId { get; set; }

    public virtual Itinerary Itinerary { get; set; } = null!;

    /// <summary>Localized title/description per supported language.</summary>
    public virtual ICollection<ActivityTranslation> Translations { get; set; } = new List<ActivityTranslation>();
}
