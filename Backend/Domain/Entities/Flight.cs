using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Flight : BaseEntity
{
    public string FlightNumber { get; set; } = null!;

    public string Airline { get; set; } = null!;

    public int DepartureCityId { get; set; }

    public int ArrivalCityId { get; set; }

    public DateTime Departure { get; set; }

    public DateTime Arrival { get; set; }

    public decimal Price { get; set; }

    public virtual City DepartureCity { get; set; } = null!;

    public virtual City ArrivalCity { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
