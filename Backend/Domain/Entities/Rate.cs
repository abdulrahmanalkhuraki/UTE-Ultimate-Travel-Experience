using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Rate
{
    public int RateValue { get; set; }

    public int UserId { get; set; }

    public int PackageId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int RateId { get; set; }

    public virtual TourPackage Package { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
