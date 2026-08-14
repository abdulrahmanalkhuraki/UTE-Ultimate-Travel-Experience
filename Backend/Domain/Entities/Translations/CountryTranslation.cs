namespace Domain.Entities.Translations;

public partial class CountryTranslation : EntityTranslation
{
    public int CountryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;
}
