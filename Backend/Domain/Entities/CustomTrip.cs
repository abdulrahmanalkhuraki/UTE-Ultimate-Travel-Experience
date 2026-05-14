using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class CustomTrip : BaseEntity
{
    public string TripName { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int NumberOfPeople { get; set; }

    public decimal Budget { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Itinerary> Itineraries { get; set; } = new List<Itinerary>();

    public virtual User User { get; set; } = null!;
}
