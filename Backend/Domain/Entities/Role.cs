using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Role : BaseEntity
{
    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
