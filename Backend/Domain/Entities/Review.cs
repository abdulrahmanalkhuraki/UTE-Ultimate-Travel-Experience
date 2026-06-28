namespace Domain.Entities;

public partial class Review : BaseEntity
{
    public string Comment { get; set; } = null!;

    public int UserId { get; set; }

    public int PackageId { get; set; }

    public virtual TourPackage Package { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
