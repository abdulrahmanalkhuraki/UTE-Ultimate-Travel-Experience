using Domain.Entities.Translations;

namespace Domain.Entities;

public partial class AttractionCategory
{
    public int CategoryId { get; set; }

    public virtual ICollection<AttractionCategoryTranslation> Translations { get; set; } = new List<AttractionCategoryTranslation>();

    public virtual ICollection<Attraction> Attractions { get; set; } = new List<Attraction>();
}
