namespace Domain.Entities.Translations;

public partial class CityTranslation : EntityTranslation
{
    public int CityId { get; set; }

    public string Name { get; set; } = null!;

    public virtual City City { get; set; } = null!;
}
