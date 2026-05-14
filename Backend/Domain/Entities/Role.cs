using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Role : BaseEntity
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
