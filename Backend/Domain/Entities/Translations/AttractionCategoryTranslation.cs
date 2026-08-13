namespace Domain.Entities.Translations;

public partial class AttractionCategoryTranslation : EntityTranslation
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual AttractionCategory Category { get; set; } = null!;
}
