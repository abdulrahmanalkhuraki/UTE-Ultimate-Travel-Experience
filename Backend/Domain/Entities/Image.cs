using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Image : BaseEntity
{
    public string ImageUrl { get; set; } = null!;

    public int AttractionId { get; set; }

    public virtual Attraction Attraction { get; set; } = null!;
}
