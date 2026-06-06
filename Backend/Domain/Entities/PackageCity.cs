using System;

namespace Domain.Entities;

/// <summary>
/// Join entity linking a <see cref="TourPackage"/> to a <see cref="City"/> it
/// visits (المناطق اللي رح تنزار). Many-to-many between programs and cities.
/// </summary>
public partial class PackageCity : BaseEntity
{
    public int PackageId { get; set; }

    public int CityId { get; set; }

    public virtual TourPackage Package { get; set; } = null!;

    public virtual City City { get; set; } = null!;
}
