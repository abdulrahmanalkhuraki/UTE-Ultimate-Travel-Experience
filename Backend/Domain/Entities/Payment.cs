using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Payment
{
    public decimal Amount { get; set; }

    public int PaymentStatus { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateOnly PaymentDate { get; set; }

    public int UserId { get; set; }

    public int PaymentId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual User User { get; set; } = null!;
}
