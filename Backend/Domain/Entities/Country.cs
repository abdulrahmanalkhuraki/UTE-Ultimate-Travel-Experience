using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Country : BaseEntity
{
    public string CountryName { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public string? Flag { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
