using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Favorite
{
    public int CompanyId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int FavoriteId { get; set; }

    public virtual TourCompany Company { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
