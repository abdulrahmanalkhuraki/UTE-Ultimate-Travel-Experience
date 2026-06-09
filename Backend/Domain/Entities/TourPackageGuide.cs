namespace Domain.Entities;

/// <summary>
/// Join entity linking a <see cref="TourPackage"/> to a <see cref="TouristGuide"/>
/// assigned to it (المرشدون المختارون للبرنامج). Many-to-many: a program may have
/// several guides, and a guide may lead several programs.
/// </summary>
public partial class TourPackageGuide : BaseEntity
{
    public int PackageId { get; set; }

    public int TouristGuideId { get; set; }

    public virtual TourPackage Package { get; set; } = null!;

    public virtual TouristGuide TouristGuide { get; set; } = null!;
}
