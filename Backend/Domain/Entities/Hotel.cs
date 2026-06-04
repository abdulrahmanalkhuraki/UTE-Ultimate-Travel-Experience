using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Hotel : BaseEntity
{
    public string HotelName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Longitude { get; set; }

    public decimal Latitude { get; set; }

    public int StarRating { get; set; }

    public decimal PricePerNight { get; set; }

    public int CityId { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual ICollection<TourPackageHotel> TourPackageHotels { get; set; } = new List<TourPackageHotel>();
}
