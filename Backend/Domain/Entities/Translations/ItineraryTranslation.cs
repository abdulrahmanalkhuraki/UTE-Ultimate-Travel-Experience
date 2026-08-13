namespace Domain.Entities.Translations;

public partial class ItineraryTranslation : EntityTranslation
{
    public int ItineraryId { get; set; }

    public string DayTitle { get; set; } = null!;

    public string? DayDescription { get; set; }

    public virtual Itinerary Itinerary { get; set; } = null!;
}
