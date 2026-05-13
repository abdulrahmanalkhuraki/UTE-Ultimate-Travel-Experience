using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Wishlist
{
    public int UserId { get; set; }

    public int AttractionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int WishlistId { get; set; }

    public virtual Attraction Attraction { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
