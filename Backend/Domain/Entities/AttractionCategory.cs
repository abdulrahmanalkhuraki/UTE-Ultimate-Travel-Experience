using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class AttractionCategory
{
    public string CategoryName { get; set; } = null!;

    public int CategoryId { get; set; }

    public int AttractionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int AttractionCategoryId { get; set; }

    public virtual Attraction Attraction { get; set; } = null!;
}
