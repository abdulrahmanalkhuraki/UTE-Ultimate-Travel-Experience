using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class AttractionActivity : BaseEntity
{
    public int ActivityId { get; set; }

    public int AttractionId { get; set; }

    public virtual Activity Activity { get; set; } = null!;

    public virtual Attraction Attraction { get; set; } = null!;
}
