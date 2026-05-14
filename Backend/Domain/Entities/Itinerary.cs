using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Itinerary : BaseEntity
{
    public int DayNumber { get; set; }

    public DateOnly DayDate { get; set; }

    public int TripId { get; set; }

    public virtual ICollection<ItineraryAttraction> ItineraryAttractions { get; set; } = new List<ItineraryAttraction>();

    public virtual CustomTrip Trip { get; set; } = null!;
}
