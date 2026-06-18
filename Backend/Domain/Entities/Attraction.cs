namespace Domain.Entities;

public partial class Attraction : BaseEntity
{
    public string AttractionName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Longitude { get; set; }

    public decimal Latitude { get; set; }

    // when both are null, attraction opens 24 hours
    public TimeOnly? OpenAt { get; set; }

    public TimeOnly? ClosedAt { get; set; }

    public decimal EntryFee { get; set; }

    public int CityId { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual ICollection<AttractionCategory> AttractionCategories { get; set; } = new List<AttractionCategory>();
}
