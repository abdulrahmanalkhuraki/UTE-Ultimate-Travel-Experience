using Domain.Enums;

namespace Domain.Entities;

public partial class TourPackageCabinClass : BaseEntity
{
    public int PackageId { get; set; }

    public FlightCabinClass CabinClass { get; set; }

    public decimal Price { get; set; }

    public bool IsDefault { get; set; }

    public virtual TourPackage Package { get; set; } = null!;
}
