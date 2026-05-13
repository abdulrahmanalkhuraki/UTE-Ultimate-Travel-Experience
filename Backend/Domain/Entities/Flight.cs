using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Flight
{
    public string FlightNumber { get; set; } = null!;

    public string Airline { get; set; } = null!;

    public string DepartureCity { get; set; } = null!;

    public string ArrivalCity { get; set; } = null!;

    public DateTime Departure { get; set; }

    public DateTime Arrival { get; set; }

    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int FlightId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
