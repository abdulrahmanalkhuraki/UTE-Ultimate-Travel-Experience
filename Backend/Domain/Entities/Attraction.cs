using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Attraction : BaseEntity
{
    public string AttractionName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Longitude { get; set; }

    public decimal Latitude { get; set; }

    public TimeOnly OpenAt { get; set; }

    public TimeOnly ClosedAt { get; set; }

    public decimal EntryFee { get; set; }

    public int CityId { get; set; }

    public virtual ICollection<AttractionActivity> AttractionActivities { get; set; } = new List<AttractionActivity>();

    public virtual ICollection<AttractionCategory> AttractionCategories { get; set; } = new List<AttractionCategory>();

    public virtual City City { get; set; } = null!;

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<PackageItineraryAttraction> PackageItineraryAttractions { get; set; } = new List<PackageItineraryAttraction>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
