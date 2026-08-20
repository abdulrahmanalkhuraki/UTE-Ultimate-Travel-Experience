namespace Domain.Entities.Translations;

public partial class TourPackageTranslation : EntityTranslation
{
    public int PackageId { get; set; }

    public string PackageName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string MeetingPoint { get; set; } = null!;

    public virtual TourPackage Package { get; set; } = null!;
}
