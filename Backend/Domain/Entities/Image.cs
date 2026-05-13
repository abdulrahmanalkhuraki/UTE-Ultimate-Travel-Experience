using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Image
{
    public string ImageUrl { get; set; } = null!;

    public int AttractionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int ImageId { get; set; }

    public virtual Attraction Attraction { get; set; } = null!;
}
