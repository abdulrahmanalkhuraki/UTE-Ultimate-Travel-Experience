using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class City
{
    public int Id { get; set; }

    public string CityName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Image { get; set; }

    public int CountryId { get; set; }

    public virtual Country Country { get; set; } = null!;
    
    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();

    public virtual ICollection<Flight> ArrivalFlights { get; set; } = new List<Flight>();

    public virtual ICollection<Flight> DepartureFlights { get; set; } = new List<Flight>();

    public virtual ICollection<Attraction> Attractions { get; set; } = new List<Attraction>();
}
