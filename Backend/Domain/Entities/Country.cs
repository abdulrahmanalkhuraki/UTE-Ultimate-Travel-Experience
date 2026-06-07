using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Country
{
    public int Id { get; set; }

    public string CountryName { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public string? Flag { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual ICollection<TourPackage> TourPackages { get; set; } = new List<TourPackage>();

    public virtual ICollection<Companion> NatinalityCompanions { get; set; } = new List<Companion>();
    public virtual ICollection<Companion> ResidentialCompanions { get; set; } = new List<Companion>();
}
