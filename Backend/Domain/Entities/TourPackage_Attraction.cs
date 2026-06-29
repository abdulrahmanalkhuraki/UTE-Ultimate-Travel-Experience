namespace Domain.Entities;

public partial class TourPackage_Attraction : BaseEntity
{
    public int PackageId { get; set; }

    public int AttractionId { get; set; }

    public virtual TourPackage Package { get; set; } = null!;

    public virtual Attraction Attraction { get; set; } = null!;
}
