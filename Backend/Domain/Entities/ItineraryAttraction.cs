using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class ItineraryAttraction : BaseEntity
{
    public int OrderNumber { get; set; }

    public TimeOnly Time { get; set; }

    public int ItineraryId { get; set; }

    public int AttractionId { get; set; }

    public virtual Attraction Attraction { get; set; } = null!;

    public virtual Itinerary Itinerary { get; set; } = null!;
}
