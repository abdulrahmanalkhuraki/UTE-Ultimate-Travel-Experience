using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Activity : BaseEntity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int Duration { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<AttractionActivity> AttractionActivities { get; set; } = new List<AttractionActivity>();
}
