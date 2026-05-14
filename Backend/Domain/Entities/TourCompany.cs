using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TourCompany : BaseEntity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Logo { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<TourPackage> TourPackages { get; set; } = new List<TourPackage>();

    public virtual User User { get; set; } = null!;
}
