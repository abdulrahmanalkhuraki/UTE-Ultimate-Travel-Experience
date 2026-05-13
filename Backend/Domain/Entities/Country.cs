using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Country
{
    public string CountryName { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public string? Flag { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int CountryId { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
