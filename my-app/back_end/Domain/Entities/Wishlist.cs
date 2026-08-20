using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Wishlist : BaseEntity
{
    public int UserId { get; set; }

    public int TourPackageId { get; set; }

    public virtual TourPackage TourPackage { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
