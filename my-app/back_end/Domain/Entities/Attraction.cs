using Domain.Entities.Translations;

namespace Domain.Entities;

public partial class Attraction : BaseEntity
{
    public int AttractionCategoryId { get; set; }

    public decimal Longitude { get; set; }

    public decimal Latitude { get; set; }

    public int CityId { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual AttractionCategory AttractionCategory { get; set; } = null!;

    public virtual ICollection<AttractionTranslation> Translations { get; set; } = new List<AttractionTranslation>();
}
