namespace Domain.Entities;

public partial class Attraction : BaseEntity
{
    public string EnAttractionName { get; set; } = null!;

    public string ArAttractionName { get; set; } = null!;

    public int AttractionCategoryId { get; set; }

    public string? Description { get; set; }

    public decimal Longitude { get; set; }

    public decimal Latitude { get; set; }

    public int CityId { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual AttractionCategory AttractionCategory { get; set; } = null!;
}
