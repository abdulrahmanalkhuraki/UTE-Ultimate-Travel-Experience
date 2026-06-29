namespace Domain.Entities;
public partial class Rate : BaseEntity
{
    public int RateValue { get; set; }

    public int UserId { get; set; }

    public int PackageId { get; set; }

    public virtual TourPackage Package { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
