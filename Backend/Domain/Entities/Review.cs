using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Review : BaseEntity
{
    public string? Comment { get; set; }

    public int UserId { get; set; }

    public int? AttractionId { get; set; }

    public int? PackageId { get; set; }

    public virtual Attraction? Attraction { get; set; }

    public virtual TourPackage? Package { get; set; }

    public virtual User User { get; set; } = null!;
}
