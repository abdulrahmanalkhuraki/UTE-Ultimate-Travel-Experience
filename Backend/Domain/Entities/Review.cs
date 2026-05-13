using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Review
{
    public string? Comment { get; set; }

    public int UserId { get; set; }

    public int? AttractionId { get; set; }

    public int? PackageId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int ReviewId { get; set; }

    public virtual Attraction? Attraction { get; set; }

    public virtual TourPackage? Package { get; set; }

    public virtual User User { get; set; } = null!;
}
