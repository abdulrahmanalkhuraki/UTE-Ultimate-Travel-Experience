using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Notification
{
    public string Message { get; set; } = null!;

    public int Type { get; set; }

    public bool IsRead { get; set; }

    public int UserId { get; set; }

    public int? BookingId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int NotificationId { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual User User { get; set; } = null!;
}
