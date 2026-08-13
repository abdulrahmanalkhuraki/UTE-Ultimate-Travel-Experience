namespace Domain.Entities.Translations;

public partial class AttractionTranslation : EntityTranslation
{
    public int AttractionId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual Attraction Attraction { get; set; } = null!;
}
