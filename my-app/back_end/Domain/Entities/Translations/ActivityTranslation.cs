namespace Domain.Entities.Translations;

public partial class ActivityTranslation : EntityTranslation
{
    public int ActivityId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public virtual Activity Activity { get; set; } = null!;
}
