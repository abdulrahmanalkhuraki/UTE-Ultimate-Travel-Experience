namespace Domain.Entities.Translations;

public partial class TouristGuideTranslation : EntityTranslation
{
    public int TouristGuideId { get; set; }

    public string Bio { get; set; } = null!;

    public virtual TouristGuide TouristGuide { get; set; } = null!;
}
