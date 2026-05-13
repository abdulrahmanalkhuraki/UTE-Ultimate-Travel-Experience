using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class City
{
    public string CityName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Image { get; set; }

    public int CountryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int CityId { get; set; }

    public virtual ICollection<Attraction> Attractions { get; set; } = new List<Attraction>();

    public virtual Country Country { get; set; } = null!;
}
