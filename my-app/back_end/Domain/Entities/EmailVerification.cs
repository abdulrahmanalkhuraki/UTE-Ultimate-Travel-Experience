using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class EmailVerification : BaseEntity
{
    public int UserId { get; set; }

    public string Code { get; set; } = null!;

    public string Purpose { get; set; } = "EmailVerification";

    public DateTime ExpiresAt { get; set; }

    public int Attempts { get; set; }

    public bool IsUsed { get; set; }

    public DateTime? UsedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
