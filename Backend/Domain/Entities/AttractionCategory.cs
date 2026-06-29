using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class AttractionCategory
{
    public int CategoryId { get; set; }

    public string EnCategoryName { get; set; } = null!;
    public string ArCategoryName { get; set; } = null!;
    public virtual ICollection<Attraction> Attractions { get; set; } = new List<Attraction>();
}
