using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// A flight cabin class made available by a <see cref="TourPackage"/>
/// (تذاكر الطيران المتاحة). Optional and multi-valued: a program may list
/// zero or more of these.
/// </summary>
public partial class TourPackageCabinClass : BaseEntity
{
    public int PackageId { get; set; }

    /// <summary>Which cabin class is offered (الدرجة).</summary>
    public FlightCabinClass CabinClass { get; set; }

    public virtual TourPackage Package { get; set; } = null!;
}
